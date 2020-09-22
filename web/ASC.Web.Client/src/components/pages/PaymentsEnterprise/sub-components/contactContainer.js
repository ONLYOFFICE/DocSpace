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

  .contact-emails {
    position: static;
    margin-bottom: 11px;
  }
  .contact-emails_link {
    color: #316daa;
  }

  @media (max-width: 632px) {
    .contact-emails {
      margin-bottom: 10px;
    }
    .contact-emails_link {
      margin-top: 3px;
    }
  }
`;

const ContactContainer = ({ salesEmail, helpUrl }) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const { t } = useTranslation("translation", { i18n });
  return (
    <StyledContactContainer>
      <Text className="contact-emails">
        {t("PurchaseQuestions")}{" "}
        <Link className="contact-emails_link" href={`mailto:${salesEmail}`}>
          {salesEmail}
        </Link>
      </Text>
      <Text className="contact-emails">
        {t("TechnicalIssues")}{" "}
        <Link
          target="_blank"
          className="contact-emails_link"
          href={`${helpUrl}`}
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

function mapStateToProps(state) {
  return {
    salesEmail: state.payments.salesEmail,
    helpUrl: state.payments.helpUrl,
  };
}
export default connect(mapStateToProps)(withRouter(ContactContainer));
