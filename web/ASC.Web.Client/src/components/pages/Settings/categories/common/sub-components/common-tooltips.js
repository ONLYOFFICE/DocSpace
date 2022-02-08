import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

const StyledLanguageTimeSettingsTooltip = styled.div`
  .font-size {
    font-size: 12px;
  }
  .bold {
    font-weight: 600;
  }
  .display {
    display: inline;
  }
  .display-link {
    display: block;
  }
`;

const LanguageTimeSettingsTooltip = ({ t }) => {
  const learnMore = t("Common:LearnMore");
  const text = t("Settings:StudioTimeLanguageSettings");
  const save = t("Common:SaveButton");
  const description = t("Settings:DescriptionTimeLanguage");

  return (
    <StyledLanguageTimeSettingsTooltip>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="LanguageTimeSettingsTooltip"
          learnMore={t("Common:LearnMore")}
          text={text}
          save={save}
          description={description}
        >
          <Text className="bold display font-size"> {{ text }}</Text>
          <Text className="display font-size">
            is a way to change the language of the whole portal for all portal
            users and to configure the time zone so that all the events of the
            ONLYOFFICE portal will be shown with the correct date and time.
          </Text>
          <Text className="font-size">{{ description }}</Text>
          <Text className="bold display font-size"> {{ save }}</Text> button at
          the bottom of the section.
          <Link
            className="display-link font-size"
            isHovered={true}
            href="https://helpcenter.onlyoffice.com/administration/configuration.aspx#CustomizingPortal_block"
          >
            {{ learnMore }}
          </Link>
        </Trans>
      </Text>
    </StyledLanguageTimeSettingsTooltip>
  );
};

export default LanguageTimeSettingsTooltip;
