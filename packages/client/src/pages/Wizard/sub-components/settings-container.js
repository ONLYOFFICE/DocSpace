import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { tablet } from "@docspace/components/utils/device";

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: min-content minmax(100px, 480px);
  grid-auto-columns: min-content;
  grid-row-gap: 12px;

  .title {
    white-space: nowrap;
  }

  .machine-name-value,
  .email-value {
    margin-left: 16px;
  }

  .drop-down {
    margin-left: 6px;
    max-width: 100%;
    .combo-button {
      width: 100%;
      .combo-button-label {
        max-width: 100%;
      }
    }
  }

  @media ${tablet} {
    width: 100%;
  }
`;

const SettingsContainer = ({
  selectLanguage,
  selectTimezone,
  languages,
  timezones,
  emailNeeded,
  email,
  emailOwner,
  t,
  machineName,
  onClickChangeEmail,
  onSelectLanguageHandler,
  onSelectTimezoneHandler,
}) => {
  const titleEmail = !emailNeeded ? <Text>{t("Common:Email")}</Text> : null;

  const contentEmail = !emailNeeded ? (
    <Link
      className="email-value"
      type="action"
      fontSize="13px"
      fontWeight="600"
      isHovered={true}
      onClick={onClickChangeEmail}
    >
      {email ? email : emailOwner}
    </Link>
  ) : null;

  return (
    <StyledContainer>
      <Text fontSize="13px">{t("Domain")}</Text>
      <Text className="machine-name-value" fontSize="13px" fontWeight="600">
        {machineName}
      </Text>

      {titleEmail}
      {contentEmail}

      <Text fontSize="13px">{t("Common:Language")}:</Text>
      <ComboBox
        className="drop-down"
        options={languages}
        selectedOption={{
          key: selectLanguage.key,
          label: selectLanguage.label,
        }}
        noBorder
        scaled={false}
        scaledOptions={false}
        size="content"
        dropDownMaxHeight={300}
        onSelect={onSelectLanguageHandler}
        fillIcon={false}
        manualWidth="250px"
      />

      <Text className="title" fontSize="13px">
        {t("Timezone")}
      </Text>
      <ComboBox
        className="drop-down"
        options={timezones}
        selectedOption={{
          key: selectTimezone.key,
          label: selectTimezone.label,
        }}
        noBorder={true}
        dropDownMaxHeight={300}
        scaled={false}
        size="content"
        onSelect={onSelectTimezoneHandler}
        textOverflow={true}
        manualWidth="280px"
      />
    </StyledContainer>
  );
};

SettingsContainer.propTypes = {
  selectLanguage: PropTypes.object.isRequired,
  selectTimezone: PropTypes.object.isRequired,
  languages: PropTypes.array.isRequired,
  timezones: PropTypes.array.isRequired,
  emailNeeded: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  t: PropTypes.func.isRequired,
  machineName: PropTypes.string.isRequired,
  email: PropTypes.string,
  onClickChangeEmail: PropTypes.func.isRequired,
  onSelectLanguageHandler: PropTypes.func.isRequired,
  onSelectTimezoneHandler: PropTypes.func.isRequired,
};

export default SettingsContainer;
