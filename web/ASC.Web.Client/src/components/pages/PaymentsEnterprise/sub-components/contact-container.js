import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, Link, utils } from "asc-web-components";

const StyledContactContainer = styled.div`
  .contact-emails {
    position: static;
    width: 920px;
    margin-bottom: 11px;
  }
  .contact-emails_link {
    color: #316daa;
  }

  @media (max-width: 632px) {
    width: 343px;

    .contact-emails {
      margin-bottom: 12px;
    }
    .contact-emails_link {
      display: block;
      margin-top: 3px;
    }
  }
`;

const ContactContainer = ({ t, salesEmail, helpUrl }) => {
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
          target="\_blank"
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
  salesEmail: PropTypes.string.isRequired,
  help: PropTypes.string.isRequired,
  t: PropTypes.func.isRequired,
};

export default ContactContainer;
