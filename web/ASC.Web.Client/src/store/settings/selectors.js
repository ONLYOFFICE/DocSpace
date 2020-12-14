import React from "react";
import { Box } from "asc-web-components";
import { format } from "react-string-format";
import { createSelector } from "reselect";

export const getUserRole = (user) => {
  if (user.isOwner) return "owner";
  else if (user.isAdmin) return "admin";
  else if (
    user.listAdminModules !== undefined &&
    user.listAdminModules.includes("people")
  )
    return "admin";
  else if (user.isVisitor) return "guest";
  else return "user";
};

export const getConsumersList = (state) => state.settings.integration.consumers;

export const getSelectedConsumer = (state) =>
  state.settings.integration.selectedConsumer;

export const getConsumerInstruction = createSelector(
  getSelectedConsumer,
  (consumer) => {
    return (
      consumer.instruction &&
      format(consumer.instruction, <Box marginProp="4px 0" />)
    );
  }
);
