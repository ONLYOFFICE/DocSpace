import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import FieldContainer from "@appserver/components/field-container";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";

import SimpleCheckbox from "./sub-components/SimpleCheckbox";
import SimpleFormField from "./sub-components/SimpleFormField";

const FieldMapping = ({ FormStore, t }) => {
  return (
    <Box>
      <Box alignItems="center" displayProp="flex" flexDirection="row">
        <Text as="h2" fontWeight={600}>
          {t("AttributeMatching")}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t("AttributeMatchingTooltip")}
        />
      </Box>

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("FirstName")}
        name="firstName"
        placeholder="givenName"
        t={t}
        tabIndex={16}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("LastName")}
        name="lastName"
        placeholder="sn"
        t={t}
        tabIndex={17}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("Common:Email")}
        name="email"
        placeholder="sn"
        t={t}
        tabIndex={18}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("Location")}
        name="location"
        placeholder="sn"
        t={t}
        tabIndex={19}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("Title")}
        name="title"
        placeholder="sn"
        t={t}
        tabIndex={20}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("Common:Phone")}
        name="phone"
        placeholder="sn"
        t={t}
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
          FormStore={FormStore}
          label={t("HideAuthPage")}
          name="hideAuthPage"
          tabIndex={22}
        />
      </FieldContainer>
    </Box>
  );
};

export default observer(FieldMapping);
