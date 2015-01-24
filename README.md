# Code-Samples
Code Samples for Integration of MailChimp and Constant Contact API integrations . The Code Samples are in C# . Net

Code samples Explanation:
The code samples sent are being used to create and manage lists of contacts and their emails using MailChimp and Constant Contact (3rd Party Api). Let us explain both one by one
1)	Integrations\ConstantContact folder contains 
 ConstantContact class which contains the methods for creating new lists, adding contacts to new / existing lists and preparing the input data before adding or updating functions are called.

2)	Integrations\MailChimp folder contains 3 classes . 

CustomMergeVars class is used to set the attributes of a new contact/email such as first name , last name etc. It was implemented to match the implementation guide line given by the Mailchimp because that’s the way they go about it.
UserContact is a class of type Contact which is passed to certain functions after setting the attributes of a contact
MailChimpLogic contains all the logic functions for adding, updating existing lists segments and adding new members or updating them in the existing lists. In Mailchimp you cannot create new lists programmatically so the concept of “Segmentation” is implemented. Both dynamic and static kinds of segments are used to cater to this problem.

3)	There are some aspx and aspx.cs files also which contain the basic design for the popups which were implemented using the above logics. 
Note: Although all the functionality files are attached but it is impossible to run these files without the whole integrated code which the company I am working with is not willing to share. But the code files above will give you an idea about my coding ability.
