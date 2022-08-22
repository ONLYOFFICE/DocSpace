import React, { useEffect, useState } from "react";

import Text from "@docspace/components/text";

import {
  StyledInfoHeaderContainer,
  StyledInfoDataContainer,
} from "./StyledBody";
import { Avatar } from "@docspace/components";
import { getUserStatus } from "SRC_DIR/helpers/people-helpers";

const SingleItem = ({ t, selection }) => {
  const [statusLabel, setStatusLabel] = React.useState("");

  const user = selection[0];

  React.useEffect(() => {
    getStatusLabel();
  }, [user, getStatusLabel]);

  const getStatusLabel = React.useCallback(() => {
    const status = getUserStatus(user);

    switch (status) {
      case "active":
        return setStatusLabel(t("Common:Active"));
      case "pending":
        return setStatusLabel(t("PeopleTranslations:PendingTitle"));
      case "disabled":
        return setStatusLabel(t("Settings:Disabled"));
    }
  }, [user]);

  return (
    <>
      <StyledInfoHeaderContainer>
        <Avatar role={user.role} size={"big"} source={user.avatar} />
        <div className="info-panel__info-text">
          <Text className={"info-text__name"} noSelect title={user.displayName}>
            {user.displayName}
          </Text>
          <Text className={"info-text__email"} noSelect title={user.email}>
            {user.email}
          </Text>
        </div>
      </StyledInfoHeaderContainer>
      <StyledInfoDataContainer>
        <div className="data__header">
          <Text className={"header__text"} noSelect title={t("Data")}>
            {t("Data")}
          </Text>
        </div>
        <div className="data__body">
          <Text className={"header__text"} noSelect title={t("Data")}>
            {t("ConnectDialog:Account")}
          </Text>
          <Text className={"header__text"} noSelect title={statusLabel}>
            {statusLabel}
          </Text>
          <Text className={"header__text"} noSelect title={t("Common:Type")}>
            {t("Common:Type")}
          </Text>
          <div>123</div>
          <Text
            className={"header__text"}
            noSelect
            title={t("People:UserStatus")}
          >
            {t("People:UserStatus")}
          </Text>
          <div>123</div>
          <Text className={"header__text"} noSelect title={t("Common:Room")}>
            {t("Common:Room")}
          </Text>
          <div>123</div>
        </div>
      </StyledInfoDataContainer>
    </>
  );
};

export default SingleItem;
