ALTER TABLE "Messaging"."UserNotification" ADD CONSTRAINT usernotification_notification_id_fk FOREIGN KEY ("NotificationId")
    REFERENCES "Messaging"."Notification" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE "Messaging"."UserNotification" ADD CONSTRAINT usernotification_user_id_fk FOREIGN KEY ("UserId")
    REFERENCES "Identity"."User" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;