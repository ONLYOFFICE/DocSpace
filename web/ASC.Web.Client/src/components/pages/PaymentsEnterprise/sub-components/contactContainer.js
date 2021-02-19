import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { withRouter } from "react-router";
import { Text, Link } from "asc-web-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const StyledContactContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(min-content, 2);
  grid-row-gap: 11px;
`;

const ContactContainer = ({ salesEmail, helpUrl }) => {
  const { t } = useTranslation("PaymentsEnterprise");
  return (
    <StyledContactContainer>
      <Text>
        {t("ContactEmail")}{" "}
        <Link href={`mailto:${salesEmail}`} color="#316daa">
          {salesEmail}
        </Link>
      </Text>

      <Text>
        {t("ContactUrl")}{" "}
        <Link target="_blank" href={`${helpUrl}`} color="#316daa">
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

export default inject(({ payments }) => {
  const { salesEmail, helpUrl } = payments;
  return {
    salesEmail,
    helpUrl,
  };
})(withRouter(observer(ContactContainer)));
