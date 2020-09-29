import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import styled from 'styled-components';
import { withTranslation } from "react-i18next";


const InfoContainer = styled.div`
  margin-bottom: 24px;
`;

const SectionBodyContent = props => {

  const {t} = props

  return (
    <InfoContainer>
        {t("UploadNewPhoto")}
    </InfoContainer>
  );
};

function mapStateToProps(state) {
  return {
  };
}

export default connect(mapStateToProps)(withTranslation()(withRouter(SectionBodyContent)));
