'use strict';

var inrule = require('./rules.js');

var func = function(event, context, callback) {

    var Mortgage = event.body;
    var session = inrule.createRuleSession();
    
    session.createEntity("Mortgage", Mortgage);
    session.applyRules(function (log) {
        if (log.hasErrors) {
        }
        Mortgage.PaymentSummary.Notifications = getNotifications(session);
        Mortgage.PaymentSummary.Validations = getValidations(session);
    });
    callback(null, Mortgage.PaymentSummary);
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