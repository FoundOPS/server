Ext.setup({
    onReady: function() {

        var form;
        
        Ext.regModel('User', {
            fields: [
                {name: 'name',     type: 'string'},
                {name: 'password', type: 'password'}
            ]
        });
        
        var formBase =  {
            scroll: 'vertical',
            url: 'Login',
            submitOnAction: false,    
            standardSubmit : true,
            items: [{
                    xtype: 'fieldset',
                    items: [
                    {
                        xtype: 'emailfield',
                        name: 'EmailAddress',
                        placeHolder: 'Email',
                        required: true,
                        useClearIcon: true
                    }, {
                        xtype: 'passwordfield',
                        name: 'Password',
                        placeHolder: 'Password',
                        required: true,
                        useClearIcon: true
                    }, {
                        xtype: 'checkboxfield',
                        name: 'RememberMe',
                        label: 'Remember Me?',
                        labelAlign: 'left',
                        labelWidth: '150px',
                        value: false
                    }]
                }
            ],
            listeners : {
                submit : function(form, result){
                    console.log('success', Ext.toArray(arguments));
                },
                exception : function(form, result){
                    console.log('failure', Ext.toArray(arguments));
                }
            },
        
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    items: [
                        { xtype: 'spacer' },
                        {
                            text: 'Login',
                            ui: 'confirm',
                            handler: function() {
                                if(formBase.user){
                                    form.updateRecord(formBase.user, true);
                                }
                                form.submit({
                                    waitMsg : {message:'Submitting', cls : 'demos-loading'}
                                });
                            }
                        }
                    ]
                }
            ]
        };
        
        if (Ext.is.Phone) {
            formBase.fullscreen = true;
        } else {
            Ext.apply(formBase, {
                autoRender: true,
                floating: false,
                modal: false,
                hideOnMaskTap: false,
                height: 240,
                width: 250
            });
        }

        form = new Ext.form.FormPanel(formBase);
        form.show();
    }
});