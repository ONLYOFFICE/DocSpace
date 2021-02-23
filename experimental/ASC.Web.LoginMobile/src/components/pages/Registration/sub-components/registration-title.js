import React from "react";
import styled from "styled-components";

import { Heading, Text, Box } from "ASC.Web.Components";

const StyledRegistrationTitle = styled(Box)`
    .registration-title, .registration-desc {
        text-align: left;
    }

    .registration-title {
        margin: 0;
        font-size: 23px;
      line-height: 28px;
      font-weight: 600;
    }

    .registration-desc {
        margin-top: 10px;
    }
`;

const RegistrationTitle = ({ t }) => {
  return (
    <StyledRegistrationTitle>
      <Heading level={1} title="Registration" className="registration-title">
        {t("RegistrationTitle")}
      </Heading>
      <Text className="registration-desc" fontSize="13px">
        {t("RegistrationDesc")}
      </Text>
    </StyledRegistrationTitle>
  );
};

export default RegistrationTitle;
