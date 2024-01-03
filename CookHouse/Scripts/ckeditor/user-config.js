/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.toolbar = 'Basic';
    config.toolbar_Basic =
    [
        ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink']
    ];

    config.removeDialogTabs = "image:advanced;link:advanced";
    config.removePlugins = "cssanim,imageresize,lightbox,slideshow,ckawesome,about";
    config.removeButtons = 'Anchor,Subscript,Superscript,Strikethrough,Source,Iframe,Flash,Scayt,Language';
    config.autoGrow_maxHeight = 400;
    config.allowedContent = true;
};
