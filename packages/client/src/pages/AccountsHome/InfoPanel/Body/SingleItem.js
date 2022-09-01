import React, { useEffect, useState } from "react";

import Text from "@docspace/components/text";
import ComboBox from "@docspace/components/combobox";
import ContextMenuButton from "@docspace/components/context-menu-button";

import {
  StyledInfoHeaderContainer,
  StyledInfoDataContainer,
} from "./StyledBody";
import { Avatar } from "@docspace/components";
import { getUserStatus } from "SRC_DIR/helpers/people-helpers";
import Badges from "../../Section/Body/Badges";

const SingleItem = ({
  t,
  selection,
  isOwner,
  isAdmin,
  changeUserType,
  userId,
  getUserContextOptions,
}) => {
  const [statusLabel, setStatusLabel] = React.useState("");

  const user = selection[0];
  const { role, statusType, options } = user;

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

  const isPending = statusType === "pending";

  const getRoomTypeLabel = React.useCallback((role) => {
    switch (role) {
      case "owner":
        return t("Common:Owner");
      case "admin":
        return t("PeopleTranslations:Administrator");
      case "manager":
        return t("PeopleTranslations:Manager");
      case "user":
        return t("Common:User");
    }
  }, []);

  const getTypesOptions = React.useCallback(() => {
    const options = [];

    const adminOption = {
      key: "admin",
      title: t("PeopleTranslations:Administrator"),
      label: t("PeopleTranslations:Administrator"),
      action: "admin",
    };
    const managerOption = {
      key: "manager",
      title: t("PeopleTranslations:Manager"),
      label: t("PeopleTranslations:Manager"),
      action: "manager",
    };
    const userOption = {
      key: "user",
      title: t("Common:User"),
      label: t("Common:User"),
      action: "user",
    };

    isOwner && options.push(adminOption);

    isAdmin && options.push(managerOption);

    options.push(userOption);

    return options;
  }, [t, isAdmin, isOwner]);

  const onTypeChange = React.useCallback(
    ({ action }) => {
      changeUserType(action, [user], t, false);
    },
    [user, changeUserType, t]
  );

  const typeLabel = getRoomTypeLabel(role);

  const getData = () => {
    const newOptions = options.filter((option) => option !== "details");

    return getUserContextOptions(t, newOptions, user);
  };

  return (
    <>
      <StyledInfoHeaderContainer isPending={isPending}>
        <Avatar
          className="avatar"
          role={user.role}
          size={"big"}
          source={user.avatar}
        />
        <div className="info-panel__info-text">
          <Text className={"info-text__name"} noSelect title={user.displayName}>
            {isPending ? user.email : user.displayName}
          </Text>
          {!isPending && (
            <Text className={"info-text__email"} noSelect title={user.email}>
              {user.email}
            </Text>
          )}
          {isPending && (
            <Badges withoutPaid={true} statusType={user.statusType} />
          )}
        </div>
        <ContextMenuButton className="context-button" getData={getData} />
      </StyledInfoHeaderContainer>
      <StyledInfoDataContainer>
        <div className="data__header">
          <Text className={"header__text"} noSelect title={t("Data")}>
            {t("InfoPanel:Data")}
          </Text>
        </div>
        <div className="data__body">
          <Text className={"info_field first-row"} noSelect title={t("Data")}>
            {t("ConnectDialog:Account")}
          </Text>
          <Text
            className={"info_data first-row"}
            fontSize={"13px"}
            fontWeight={600}
            noSelect
            title={statusLabel}
          >
            {statusLabel}
          </Text>
          <Text className={"info_field"} noSelect title={t("Common:Type")}>
            {t("Common:Type")}
          </Text>
          {((isOwner && role !== "owner") ||
            (isAdmin && !isOwner && role !== "admin")) &&
          statusType !== "disabled" &&
          userId !== user.id ? (
            <ComboBox
              className="type-combobox"
              selectedOption={getTypesOptions().find(
                (option) => option.key === role
              )}
              options={getTypesOptions()}
              onSelect={onTypeChange}
              scaled={false}
              size="content"
              displaySelectedOption
              modernView
            />
          ) : (
            <Text
              type="page"
              title={typeLabel}
              fontSize="13px"
              fontWeight={600}
              truncate
              noSelect
            >
              {typeLabel}
            </Text>
          )}
          <Text className={"info_field"} noSelect title={t("UserStatus")}>
            {t("UserStatus")}
          </Text>
          <Text
            className={"info_data first-row"}
            fontSize={"13px"}
            fontWeight={600}
            noSelect
            title={statusLabel}
          >
            {t("SmartBanner:Price")}
          </Text>
          {/* <Text className={"info_field"} noSelect title={t("Common:Room")}>
            {t("Common:Room")}
          </Text>
          <div>Rooms list</div> */}
        </div>
      </StyledInfoDataContainer>
    </>
  );
};

export default SingleItem;
