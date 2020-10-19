import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, Link } from "asc-web-components";
import { useTranslation } from "react-i18next";
import { createI18N } from "../../../../helpers/i18n";
import { utils } from "asc-web-common";
const { changeLanguage } = utils;

const i18n = createI18N({
  page: "PaymentsEnterprise",
  localesPath: "pages/PaymentsEnterprise",
});

const StyledContactContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: repeat(min-content, 2);
  grid-row-gap: 11px;
`;

const ContactContainer = ({ salesEmail, helpUrl }) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const { t } = useTranslation("translation", { i18n });
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

function mapStateToProps(state) {
  return {
    salesEmail: state.payments.salesEmail,
    helpUrl: state.payments.helpUrl,
  };
}
export default connect(mapStateToProps)(withRouter(ContactContainer));
