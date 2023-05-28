# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import Select
from selenium.common.exceptions import NoSuchElementException
from selenium.common.exceptions import NoAlertPresentException
import unittest, time, re

class Test6(unittest.TestCase):
    def setUp(self):
        self.driver = webdriver.Chrome(executable_path=r'')
        self.driver.implicitly_wait(30)
        self.verificationErrors = []
        self.accept_next_alert = True
    
    def test_6(self):
        driver = self.driver
        driver.get("http://192.168.69.64:5000/")
        driver.find_element(By.XPATH, "//a[@href='/login']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").send_keys("wedy628@gmail.com")
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//button[@type='submit']").click()
        assert driver.find_element(By.XPATH, "//a[@href='/logout']").is_displayed()
    
    def test_7(self):
        driver = self.driver
        driver.get("http://192.168.69.64:5000/")
        driver.find_element(By.XPATH, "//a[@href='/login']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").send_keys("Radoslav.Titko@stud.vilniustech.lt")
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").send_keys("asdqweertasdfdfb")
        driver.find_element(By.XPATH, "//button[@type='submit']").click()
        assert driver.find_element(By.XPATH, "//a[@href='/login']").is_displayed()

    def test_8(self):
        driver = self.driver
        driver.get("http://192.168.69.64:5000/")
        driver.find_element(By.XPATH, "//a[@href='/register']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").send_keys("Radoslav")
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").send_keys("radoslav.titko@stud.vilniustech.lt")
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//button[@type='submit']").click()
        assert driver.find_element(By.XPATH, "//p[text()='Registration succeeded.']").is_displayed()

    def test_9(self):
        driver = self.driver
        driver.get("http://192.168.69.64:5000/")
        driver.find_element(By.XPATH, "//a[@href='/register']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Name']").send_keys("Radoslav")
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Email']").send_keys("radoslav.titko@stud.vilniustech.lt")
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_Password']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").click()
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").clear()
        driver.find_element(By.XPATH, "//input[@id='registerModel_ConfirmPassword']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//button[@type='submit']").click()
        assert driver.find_element(By.XPATH, "//div[@class='text-danger validation-summary-errors']").is_displayed()

    def test_10(self):
        driver = self.driver
        driver.get("http://192.168.69.64:5000/")
        driver.find_element(By.XPATH, "//a[@href='/login']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").click()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Username']").send_keys("wedy628@gmail.com")
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").clear()
        driver.find_element(By.XPATH, "//input[@id='loginModel_Password']").send_keys("qweasdzxc123")
        driver.find_element(By.XPATH, "//button[@type='submit']").click()
        driver.find_element(By.XPATH, "//a[@href='/logout']").click()
        assert driver.find_element(By.XPATH, "//a[@href='/login']").is_displayed()
    
    def is_element_present(self, how, what):
        try: self.driver.find_element(by=how, value=what)
        except NoSuchElementException as e: return False
        return True
    
    def is_alert_present(self):
        try: self.driver.switch_to_alert()
        except NoAlertPresentException as e: return False
        return True
    
    def close_alert_and_get_its_text(self):
        try:
            alert = self.driver.switch_to_alert()
            alert_text = alert.text
            if self.accept_next_alert:
                alert.accept()
            else:
                alert.dismiss()
            return alert_text
        finally: self.accept_next_alert = True
    
    def tearDown(self):
        self.driver.quit()
        self.assertEqual([], self.verificationErrors)

if __name__ == "__main__":
    unittest.main()
