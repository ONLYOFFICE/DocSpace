import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, Link } from "asc-web-components";
const supportLinkPurchaseQuestions = "sales@onlyoffice.com";
const supportLinkTechnicalIssues = "https://helpdesk.onlyoffice.com";

const StyledContactContainer = styled.div`
  .contact-emails {
    position: static;
    width: 920px;
    margin-bottom: 11px;
  }
  .contact-emails_link {
    color: #316daa;
  }
`;

const ContactContainer = ({ t }) => {
  return (
    <StyledContactContainer>
      <Text className="contact-emails">
        {t("PurchaseQuestions")}{" "}
        <Link
          className="contact-emails_link"
          href={`mailto:${supportLinkPurchaseQuestions}`}
        >
          {supportLinkPurchaseQuestions}
        </Link>
      </Text>
      <Text className="contact-emails">
        {t("TechnicalIssues")}{" "}
        <Link
          className="contact-emails_link"
          href={`${supportLinkTechnicalIssues}`}
        >
          {supportLinkTechnicalIssues}
        </Link>
      </Text>
    </StyledContactContainer>
  );
};

ContactContainer.propTypes = {
  t: PropTypes.func.isRequired,
};

export default ContactContainer;
