import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import { IconButton, utils } from "asc-web-components";
import { Headline } from "asc-web-common";
import { withTranslation } from "react-i18next";
import { department } from "./../../../../../helpers/customNames";
import { resetGroup } from "../../../../../store/group/actions";
import styled from "styled-components";

const Wrapper = styled.div`
  display: flex;
  align-items: center;

  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

const HeaderContainer = styled(Headline)`
  margin-left: 16px;
  max-width: calc(100vw - 430px);

  @media ${utils.device.tablet} {
    max-width: calc(100vw - 64px);
  }
`;

class SectionHeaderContent extends React.Component {
  onClickBack = () => {
    const { history, settings, resetGroup } = this.props;

    resetGroup();
    history.push(settings.homepage);
  };

  render() {
    const { group, t } = this.props;
    const headerText = group
      ? t("CustomEditDepartment", { department })
      : t("CustomNewDepartment", { department });
    return (
      <Wrapper>
        <IconButton
          iconName="ArrowPathIcon"
          size="16"
          color="#A3A9AE"
          hoverColor="#657077"
          isFill={true}
          onClick={this.onClickBack}
          className="arrow-button"
        />
        <HeaderContainer type="content" truncate={true}>
          {headerText}
        </HeaderContainer>
      </Wrapper>
    );
  }
}

SectionHeaderContent.propTypes = {
  group: PropTypes.object,
  history: PropTypes.object.isRequired
};

SectionHeaderContent.defaultProps = {
  group: null
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup
  };
}

export default connect(
  mapStateToProps,
  { resetGroup }
)(withTranslation()(withRouter(SectionHeaderContent)));
