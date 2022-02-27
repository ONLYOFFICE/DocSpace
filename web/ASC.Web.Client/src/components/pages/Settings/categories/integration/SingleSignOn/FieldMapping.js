import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import FieldContainer from "@appserver/components/field-container";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";

import SimpleCheckbox from "./sub-components/SimpleCheckbox";
import SimpleFormField from "./sub-components/SimpleFormField";

const FieldMapping = () => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  return (
    <Box>
      <Box
        alignItems="center"
        displayProp="flex"
        flexDirection="row"
        fontSize="14px"
        marginProp="22px 0 14px 0"
      >
        <Text as="h2" fontSize="14px" fontWeight={600}>
          {t("AttributeMatching")}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t("AttributeMatchingTooltip")}
        />
      </Box>

      <SimpleFormField
        labelText={t("FirstName")}
        name="firstName"
        placeholder="givenName"
        tabIndex={16}
      />

      <SimpleFormField
        labelText={t("LastName")}
        name="lastName"
        placeholder="sn"
        tabIndex={17}
      />

      <SimpleFormField
        labelText={t("Common:Email")}
        name="email"
        placeholder="sn"
        tabIndex={18}
      />

      <SimpleFormField
        labelText={t("Location")}
        name="location"
        placeholder="sn"
        tabIndex={19}
      />

      <SimpleFormField
        labelText={t("Title")}
        name="title"
        placeholder="sn"
        tabIndex={20}
      />

      <SimpleFormField
        labelText={t("Common:Phone")}
        name="phone"
        placeholder="sn"
        tabIndex={21}
      />

      <FieldContainer
        inlineHelpButton
        isVertical
        labelText={t("AdvancedSettings")}
        place="top"
        tooltipContent={t("AdvancedSettingsTooltip")}
      >
        <SimpleCheckbox
          label={t("HideAuthPage")}
          name="hideAuthPage"
          tabIndex={22}
        />
      </FieldContainer>
    </Box>
  );
};

export default observer(FieldMapping);
