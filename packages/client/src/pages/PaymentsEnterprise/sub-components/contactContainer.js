import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

const StyledContactContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(min-content, 2);
  grid-row-gap: 11px;
`;

const ContactContainer = ({ salesEmail, helpUrl, theme }) => {
  const { t } = useTranslation("PaymentsEnterprise");
  return (
    <StyledContactContainer>
      <Text>
        {t("ContactEmail")}{" "}
        <Link
          href={`mailto:${salesEmail}`}
          color={theme.client.paymentsEnterprise.linkColor}
        >
          {salesEmail}
        </Link>
      </Text>

      <Text>
        {t("ContactUrl")}{" "}
        <Link
          target="_blank"
          href={`${helpUrl}`}
          color={theme.client.paymentsEnterprise.linkColor}
        >
          {helpUrl}
        </Link>
      </Text>
    </StyledContactContainer>
  );
};

ContactContainer.propTypes = {
  salesEmail: PropTypes.string,
  helpUrl: PropTypes.string,
};

export default inject(({ payments, auth }) => {
  const { salesEmail, helpUrl } = payments;
  return {
    salesEmail,
    helpUrl,
    theme: auth.settingsStore.theme,
  };
})(withRouter(observer(ContactContainer)));
