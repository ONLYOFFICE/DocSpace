import React, { useEffect, useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { PageLayout } from "asc-web-components";
import {ArticleHeaderContent, ArticleBodyContent} from '../../Article';
import {SectionHeaderContent, SectionBodyContent} from './Section';
import { getUser } from '../../../utils/api';

const ProfileAction = (props) => {
  console.log("Profile userId", props.match.params.userId);
  const { userId } = props.match.params;
  const { history, auth } = props;
  const [profile, setProfile] = useState(auth.user);

  useEffect(() =>{
    if(userId === "@self") {
      setProfile(auth.user);
    }
    else {
      getUser(userId)
      .then((res) => { 
        setProfile(res.data.response);
      });
    }
  }, []);

  return (
      <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent profile={profile} history={history} />}
          sectionBodyContent={<SectionBodyContent profile={profile} history={history} /> }
      />
  );
};

ProfileAction.propTypes = {
  auth: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired
};

function mapStateToProps(state) {
  return {
    auth: state.auth
  };
}

export default connect(mapStateToProps)(withRouter(ProfileAction));