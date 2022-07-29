# Kindred.ConsoleApp.LogoUpdater
### This app updates the brand logo information fro coupons.de but can change code to work with any affiliate network brands.
### App looks at a file in Data folder which has latest imported data file from blob storage import data and gets the NULL logos for coupons.de from DB
### it does the merchant id comparions to gets the correct logo url and updates it in the DB

#How to run
### Copy the lastest import data from blob storage Coupon4U 
### Update the connection staring for an environemnt you are looking to update
### Run the app