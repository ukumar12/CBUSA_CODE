CKEDITOR.addTemplates('default',{imagesPath:CKEDITOR.getUrl(CKEDITOR.plugins.getPath('templates')+'templates/images/'),templates:[{
  "title": "Image and Title",
  "description": "One main image with a title and text that surround the image.",
  "image": "template1.gif",
  "html": "<h3><img style=\"margin-right: 10px\" height=\"100\" width=\"100\" align=\"left\"/>Type the title here</h3><p>Type the text here123</p>"
},{
  "title": "Strange Template",
  "image": "template2.gif",
  "description": "A template that defines two colums, each one with a title, and some text.",
  "html": "<table cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%\" border=\"0\"><tr><td style=\"width:50%\"><h3>Title 1</h3></td><td></td><td style=\"width:50%\"><h3>Title 2</h3></td></tr><tr><td>Text 1</td><td></td><td>Text 2</td></tr></table><p>More text goes here.</p>"
}]});
