import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { PageLayout } from "asc-web-components";
import {ArticleHeaderContent, ArticleBodyContent} from '../../Article';
import {SectionHeaderContent, SectionBodyContent} from './Section';

const Profile = (props) => {
    console.log("Profile userId", props.match.params.userId);
    return (
        <PageLayout
            articleHeaderContent={<ArticleHeaderContent />}
            articleBodyContent={<ArticleBodyContent />}
            sectionHeaderContent={<SectionHeaderContent /> }
            sectionBodyContent={<SectionBodyContent /> }
        />
    );
};

Profile.propTypes = {
    history: PropTypes.object.isRequired,
    match: PropTypes.object.isRequired,
    auth: PropTypes.object
  };

  function mapStateToProps(state) {
    return {
      auth: state.auth
    };
  }

export default connect(mapStateToProps)(withRouter(Profile));