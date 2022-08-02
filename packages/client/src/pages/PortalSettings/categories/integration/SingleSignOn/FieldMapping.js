import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import FieldContainer from "@docspace/components/field-container";
import HelpButton from "@docspace/components/help-button";
import Text from "@docspace/components/text";

import Checkbox from "@docspace/components/checkbox";
import SsoFormField from "./sub-components/SsoFormField";

const FieldMapping = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    firstName,
    lastName,
    email,
    location,
    title,
    phone,
    hideAuthPage,
    enableSso,
    setCheckbox,
    isLoadingXml,
    firstNameHasError,
    lastNameHasError,
    emailHasError,
    locationHasError,
    titleHasError,
    phoneHasError,
  } = props;

  return (
    <Box>
      <Box
        alignItems="center"
        displayProp="flex"
        flexDirection="row"
        fontSize="14px"
        marginProp="24px 0 16px 0"
      >
        <Text as="h2" fontSize="14px" fontWeight={600} noSelect>
          {t("AttributeMatching")}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t("AttributeMatchingTooltip")}
        />
      </Box>

      <SsoFormField
        labelText={t("FirstName")}
        name="firstName"
        placeholder="givenName"
        tabIndex={16}
        value={firstName}
        hasError={firstNameHasError}
      />

      <SsoFormField
        labelText={t("LastName")}
        name="lastName"
        placeholder="sn"
        tabIndex={17}
        value={lastName}
        hasError={lastNameHasError}
      />

      <SsoFormField
        labelText={t("Common:Email")}
        name="email"
        placeholder="sn"
        tabIndex={18}
        value={email}
        hasError={emailHasError}
      />

      <SsoFormField
        labelText={t("Location")}
        name="location"
        placeholder="sn"
        tabIndex={19}
        value={location}
        hasError={locationHasError}
      />

      <SsoFormField
        labelText={t("Title")}
        name="title"
        placeholder="sn"
        tabIndex={20}
        value={title}
        hasError={titleHasError}
      />

      <SsoFormField
        labelText={t("Common:Phone")}
        name="phone"
        placeholder="sn"
        tabIndex={21}
        value={phone}
        hasError={phoneHasError}
      />

      <FieldContainer
        className="advanced-block"
        inlineHelpButton
        isVertical
        labelText={t("AdvancedSettings")}
        place="top"
        tooltipContent={t("AdvancedSettingsTooltip")}
      >
        <Checkbox
          className="checkbox-input"
          label={t("HideAuthPage")}
          name="hideAuthPage"
          tabIndex={22}
          isChecked={hideAuthPage}
          isDisabled={!enableSso || isLoadingXml}
          onChange={setCheckbox}
        />
      </FieldContainer>
    </Box>
  );
};

export default inject(({ ssoStore }) => {
  const {
    firstName,
    lastName,
    email,
    location,
    title,
    phone,
    hideAuthPage,
    enableSso,
    setCheckbox,
    isLoadingXml,
    firstNameHasError,
    lastNameHasError,
    emailHasError,
    locationHasError,
    titleHasError,
    phoneHasError,
  } = ssoStore;

  return {
    firstName,
    lastName,
    email,
    location,
    title,
    phone,
    hideAuthPage,
    enableSso,
    setCheckbox,
    isLoadingXml,
    firstNameHasError,
    lastNameHasError,
    emailHasError,
    locationHasError,
    titleHasError,
    phoneHasError,
  };
})(observer(FieldMapping));
