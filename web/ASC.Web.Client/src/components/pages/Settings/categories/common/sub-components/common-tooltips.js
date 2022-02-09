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

const LanguageTimeSettingsTooltip = ({ t, theme }) => {
  const learnMore = t("Common:LearnMore");
  const text = t("Settings:StudioTimeLanguageSettings");
  const save = t("Common:SaveButton");

  return (
    <StyledLanguageTimeSettingsTooltip>
      <Text className="font-size">
        <Trans ns="Settings" i18nKey="LanguageTimeSettingsTooltip" text={text}>
          <Text className="bold display font-size">{{ text }}</Text>{" "}
        </Trans>
      </Text>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="LanguageTimeSettingsTooltipDescription"
          learnMore={learnMore}
          save={save}
        >
          {" "}
          <Text className="bold display font-size"> {{ save }}</Text>{" "}
          <Link
            color={theme.studio.settings.common.linkColorHelp}
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
