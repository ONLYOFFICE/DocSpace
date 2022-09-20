import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

const StyledContactContainer = styled.div`
  display: flex;
  width: max-content;
  p {
    margin-right: 4px;
  }
`;

const ContactContainer = ({ t, salesEmail, theme }) => {
  return (
    <StyledContactContainer>
      <Text noSelect fontWeight={600}>
        {t("ContactUs")}
      </Text>

      <Link
        fontWeight={600}
        href={`mailto:${salesEmail}`}
        color={theme.client.payments.linkColor}
      >
        {salesEmail}
      </Link>
    </StyledContactContainer>
  );
};

export default inject(({ payments, auth }) => {
  const { salesEmail } = payments;
  return {
    salesEmail,
    theme: auth.settingsStore.theme,
  };
})(withRouter(observer(ContactContainer)));
