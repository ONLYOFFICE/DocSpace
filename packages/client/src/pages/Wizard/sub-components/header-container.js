import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Box from "@docspace/components/box";
import Heading from "@docspace/components/heading";
import Text from "@docspace/components/text";
import { tablet } from "@docspace/components/utils/device";

const StyledHeaderContainer = styled(Box)`
  width: 100%;

  .wizard-title {
    text-align: center;
    font-weight: 600;
    font-size: 32px;
    line-height: 36px;
    margin: 0;
  }

  .wizard-desc {
    text-align: center;
    margin-top: 9px;
  }

  @media ${tablet} {
    .wizard-title,
    .wizard-desc {
      text-align: left;
    }
  }

  @media (max-width: 520px) {
    .wizard-title {
      font-size: 23px;
      line-height: 28px;
    }
  }
`;

const HeaderContainer = ({ t }) => {
  return (
    <StyledHeaderContainer>
      <Heading level={1} title="Wizard" className="wizard-title">
        {t("WelcomeTitle")}
      </Heading>
      <Text className="wizard-desc" fontSize="13px">
        {t("Desc")}
      </Text>
    </StyledHeaderContainer>
  );
};

HeaderContainer.propTypes = {
  t: PropTypes.func.isRequired,
};

export default HeaderContainer;
