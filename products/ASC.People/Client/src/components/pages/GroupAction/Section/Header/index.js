import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import { IconButton, Header } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { department } from "./../../../../../helpers/customNames";
import { resetGroup } from "../../../../../store/group/actions";

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px"
};

class SectionHeaderContent extends React.Component {
  onBackClick = () => {
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
      <div style={wrapperStyle}>
        <IconButton
          iconName={"ArrowPathIcon"}
          size="16"
          onClick={this.onBackClick}
        />
        <Header type="content" style={textStyle}>{headerText}</Header>
      </div>
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
