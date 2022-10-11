import React from "react";
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
  selfId,
}) => {
  const [statusLabel, setStatusLabel] = React.useState("");

  const { role, statusType, options } = selection;

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
    }
  }, [selection]);

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
      changeUserType(action, [selection], t, false);
    },
    [selection, changeUserType, t]
  );

  const typeLabel = getRoomTypeLabel(role);

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
          {role !== "guest" && ( //TODO: delete this condition after remove guest type
            <>
              <Text className={"info_field"} noSelect title={t("Common:Type")}>
                {t("Common:Type")}
              </Text>
              <>
                {((isOwner && role !== "owner") ||
                  (isAdmin && !isOwner && role !== "admin")) &&
                statusType !== "disabled" &&
                selfId !== selection.id ? (
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
              </>
            </>
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
      </StyledAccountContent>
    </>
  );
};

export default withTranslation([
  "People",
  "InfoPanel",
  "ConnectDialog",
  "Common",
  "PeopleTranslations",
  "Settings",
  "SmartBanner",
  "DeleteProfileEverDialog",
  "Translations",
])(withLoader(Accounts)(<Loaders.InfoPanelViewLoader view="accounts" />));
