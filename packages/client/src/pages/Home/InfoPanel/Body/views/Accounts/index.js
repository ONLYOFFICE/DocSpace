import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

import Text from "@docspace/components/text";
import ComboBox from "@docspace/components/combobox";

import { getUserStatus } from "SRC_DIR/helpers/people-helpers";
import { StyledAccountContent } from "../../styles/accounts";

const Accounts = ({
  t,
  selection,
  isOwner,
  isAdmin,
  changeUserType,
  canChangeUserType,
  selfId,
}) => {
  const [statusLabel, setStatusLabel] = React.useState("");

  const { role, id, isVisitor } = selection;

  React.useEffect(() => {
    getStatusLabel();
  }, [selection, getStatusLabel]);

  const getStatusLabel = React.useCallback(() => {
    const status = getUserStatus(selection);
    switch (status) {
      case "active":
        return setStatusLabel(t("Common:Active"));
      case "pending":
        return setStatusLabel(t("PeopleTranslations:PendingTitle"));
      case "disabled":
        return setStatusLabel(t("Settings:Disabled"));
      default:
        return setStatusLabel(t("Common:Active"));
    }
  }, [selection]);

  const getUserTypeLabel = React.useCallback((role) => {
    switch (role) {
      case "owner":
        return t("Common:Owner");
      case "admin":
        return t("Common:DocSpaceAdmin");
      case "manager":
        return t("Common:RoomAdmin");
      case "user":
        return t("Common:User");
    }
  }, []);

  const getTypesOptions = React.useCallback(() => {
    const options = [];

    const adminOption = {
      key: "admin",
      title: t("Common:DocSpaceAdmin"),
      label: t("Common:DocSpaceAdmin"),
      action: "admin",
    };
    const managerOption = {
      key: "manager",
      title: t("Common:RoomAdmin"),
      label: t("Common:RoomAdmin"),
      action: "manager",
    };
    const userOption = {
      key: "user",
      title: t("Common:User"),
      label: t("Common:User"),
      action: "user",
    };

    isOwner && options.push(adminOption);

    options.push(managerOption);

    isVisitor && options.push(userOption);

    return options;
  }, [t, isAdmin, isOwner, isVisitor]);

  const onTypeChange = React.useCallback(
    ({ action }) => {
      changeUserType(action, [selection], t, false);
    },
    [selection, changeUserType, t]
  );

  const typeLabel = getUserTypeLabel(role);

  const renderTypeData = () => {
    const typesOptions = getTypesOptions();

    const combobox = (
      <ComboBox
        className="type-combobox"
        selectedOption={
          typesOptions.find((option) => option.key === role) || {}
        }
        options={typesOptions}
        onSelect={onTypeChange}
        scaled={false}
        size="content"
        displaySelectedOption
        modernView
      />
    );

    const text = (
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
    );

    const status = getUserStatus(selection);

    const canChange = canChangeUserType({ ...selection, statusType: status });

    return canChange ? combobox : text;
  };

  const typeData = renderTypeData();

  const statusText = isVisitor ? t("SmartBanner:Price") : t("Common:Paid");

  return (
    <>
      <StyledAccountContent>
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
          {typeData}

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
            {statusText}
          </Text>
          {/* <Text className={"info_field"} noSelect title={t("Common:Room")}>
            {t("Common:Room")}
          </Text>
          <div>Rooms list</div> */}
        </div>
      </StyledAccountContent>
    </>
  );
};

export default inject(({ auth, peopleStore }) => {
  const { isOwner, isAdmin, id: selfId } = auth.userStore.user;
  const { changeType: changeUserType } = peopleStore;

  return {
    isOwner,
    isAdmin,
    changeUserType,
    selfId,
  };
})(
  withTranslation([
    "People",
    "InfoPanel",
    "ConnectDialog",
    "Common",
    "PeopleTranslations",
    "People",
    "Settings",
    "SmartBanner",
    "DeleteProfileEverDialog",
    "Translations",
  ])(
    withLoader(observer(Accounts))(
      <Loaders.InfoPanelViewLoader view="accounts" />
    )
  )
);
