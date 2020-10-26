# import the necessary packages
from skimage.measure import compare_ssim as ssim
import matplotlib.pyplot as plt
import numpy as np
import cv2
import sys
import time as thread
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.keys import Keys
import os, shutil
import unittest


class ImageComapre(unittest.TestCase):
    img1_ori = r'C:\Users\sello\Desktop\Report.png'
    img2_edit = r'C:\Users\sello\Desktop\Report-Copy.png'
    img2_Contr = r'C:\Users\sello\Desktop\Report-Contr.png'
    def setUp(self):
        self.driver = webdriver.Firefox()
        pass
    def CleanFolder(folder):
            for filename in os.listdir(folder):
                file_path = os.path.join(folder, filename)
                try:
                    if os.path.isfile(file_path) or os.path.islink(file_path):
                        os.unlink(file_path)
                    elif os.path.isdir(file_path):
                        shutil.rmtree(file_path)
                    Cleaned = True
                except Exception as e:
                    print('Failed to delete %s. Reason: %s' % (file_path, e))
            pass
    def mse(imageA, imageB):
        # the 'Mean Squared Error' between the two images is the
        # sum of the squared difference between the two images;
        # NOTE: the two images must have the same dimension
        err = np.sum((imageA.astype("float") - imageB.astype("float")) ** 2)
        err /= float(imageA.shape[0] * imageA.shape[1])
        # return the MSE, the lower the error, the more "similar"
        # the two images are
        return err
    def compare_images(imageA, imageB, title):
        # compute the mean squared error and structural similarity
        # index for the images
        m = mse(imageA, imageB)
        s = ssim(imageA, imageB)
        # setup the figure
        fig = plt.figure(title)
        plt.suptitle("MSE: %.2f, SSIM: %.2f" % (m, s))
        # show first image
        ax = fig.add_subplot(1, 2, 1)
        plt.imshow(imageA, cmap = plt.cm.gray)
        plt.axis("off")
        # show the second image
        ax = fig.add_subplot(1, 2, 2)
        plt.imshow(imageB, cmap = plt.cm.gray)
        plt.axis("off")
        # show the images
        plt.show()
    def CheckBoundary(driver):
        #driver = webdriver.Firefox()
        ExpectedOutput = r'C:\Users\sello\source\repos\AutoCISHealthCheck\PythonApplication1\ExpectedScreenshots\EpectedBoundary.png'
        CapturedOutput = r'C:\Users\sello\source\repos\AutoCISHealthCheck\PythonApplication1\CapturedScreenshots\CapturedOutput.png'
        DocPath = r'C:\Users\sello\source\repos\AutoCISHealthCheck\PythonApplication1\CapturedScreenShots'
        try:
            Similar=None
            CleanFolder(DocPath)
            driver.save_screenshot(CapturedOutput)

            ExpectedImg = cv2.imread(ExpectedOutput)
            ActualImg = cv2.imread(CapturedOutput)

            Exp = cv2.cvtColor(ExpectedImg, cv2.COLOR_BGR2GRAY) #expected output
            Act = cv2.cvtColor(ActualImg, cv2.COLOR_BGR2GRAY) #Actual Output/ captured image

            m = mse(Exp,Act)
            s = ssim(Exp, Act)
            Captured = True


            if int(m) == 0 and int(s) ==1 :
                Similar = True
            else:
                Similar = False
        except Exception as e:
                print(e)
                Captured = False
    
        return Similar
    def ExplicitWait(driver,ElementXpath):
        try:
            #ExpectedElement = WebDriverWait(driver,10).until(EC.presence_of_element_located(By.XPATH(ElementXpath)))
            ExpectedElement = WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.XPATH, ElementXpath)))
            thread.sleep(2)
        except :
            ExpectedElement = None

        return ExpectedElement
    def ReadImages(Expected,Actual):
        # load the images -- the Expected, And the Acual image,
        path1 = r'C:\Users\sello\Desktop\Report.png'
        original = cv2.imread(img1_ori)
        contrast = cv2.imread(img2_Contr)
        shopped = cv2.imread(img2_edit)

        # convert the images to grayscale
        original = cv2.cvtColor(original, cv2.COLOR_BGR2GRAY)
        contrast = cv2.cvtColor(contrast, cv2.COLOR_BGR2GRAY)
        shopped = cv2.cvtColor(shopped, cv2.COLOR_BGR2GRAY)

        # initialize the figure
        fig = plt.figure("Images")
        images = ("Original", original), ("Contrast", contrast), ("Photoshopped", shopped)
        # loop over the images
        for (i, (name, image)) in enumerate(images):
            # show the image
            ax = fig.add_subplot(1, 3, i + 1)
            ax.set_title(name)
            plt.imshow(image, cmap = plt.cm.gray)
            plt.axis("off")
        # show the figure
        plt.show()
        # compare the images
        compare_images(original, original, "Original vs. Original")
        compare_images(original, contrast, "Original vs. Contrast")
        compare_images(original, shopped, "Original vs. Photoshopped")
    def tearDown(self):
        self.driver.close()
        pass
    def Validate_Report(self):
        driver = self.driver
        driver.get("http://10.1.15.226/#/login")
        InternalUserbtn = driver.find_element_by_xpath("/html/body/az-root/az-login/div/div/div/div/form/div[1]/div[1]/label")
        InternalUserbtn.click()
    obj = ImageComapre()
    obj.Validate_Report()

driver = webdriver.Firefox()







