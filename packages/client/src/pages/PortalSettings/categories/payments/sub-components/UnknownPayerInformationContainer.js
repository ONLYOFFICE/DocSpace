import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { Link } from "@docspace/components";

const StyledContainer = styled.div`
  .change-payer {
    margin-left: 3px;
  }
`;

const UnknownPayerInformationContainer = ({
  style,
  theme,
  email,
  t,
  isLinkAvailable,
  accountLink,
}) => {
  return (
    <StyledContainer style={style} theme={theme}>
      <div>
        <Text as="span" fontSize="13px" isBold>
          {email}
          {"."}
        </Text>
        {isLinkAvailable ? (
          <Link
            noSelect
            fontWeight={600}
            href={accountLink}
            className="change-payer"
            color={theme.client.payments.linkColor}
          >
            {t("ChangePayer")}
          </Link>
        ) : (
          <Text as="span" fontSize="13px" isBold className="change-payer">
            {t("OwnerCanChangePayer")}
            {"."}
          </Text>
        )}
      </div>
    </StyledContainer>
  );
};

export default UnknownPayerInformationContainer;
