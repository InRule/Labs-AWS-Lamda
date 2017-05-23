'use strict';

var inrule = require('./{0}');

var func = function(event, context, callback) {

    var {1} = event.body;
    var session = inrule.createRuleSession();
    
    session.createEntity("{1}", {1});
    session.applyRules(function (log) {
        if (log.hasErrors) {
        }
        {2}.Notifications = getNotifications(session);
        {2}.Validations = getValidations(session);
    });
    callback(null, {2});
};

exports.handler = func;


function getNotifications(session) {
    // write all notifications to a string with line breaks
    var text = '';
    var notifications = session.getActiveNotifications();
    for (var i = 0; i < notifications.length; i++) {
        text += notifications[i].message;
        text += '\n';
    }
    return text;
}

function getValidations(session) {
    // write all notifications to a string with line breaks
    var text = '';
    var validations = session.getActiveValidations();
    for (var i = 0; i < validations.length; i++) {
        text += validations[i].message;
        text += '\n';
    }
    return text;
}