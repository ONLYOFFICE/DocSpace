import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Text, Link, utils } from "asc-web-components";

const { tablet, mobile } = utils.device;
const StyledContactContainer = styled.div`
  .contact-emails {
    position: static;
    width: 920px;
    margin-bottom: 11px;
  }
  .contact-emails_link {
    color: #316daa;
  }

  @media ${mobile} {
    width: 343px;
    .contact-emails_link {
      display: block;
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
  t: PropTypes.func.isRequired,
};

export default ContactContainer;
