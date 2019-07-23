import React, { useEffect, useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { getUser } from '../../../utils/api';

const getUrlParam = (inputString, paramName, defaultValue) => {
  var regex = new RegExp(`${paramName}=([^&]+)`);
  var res = regex.exec(inputString);
  return res && res[1] ? res[1] : defaultValue;
}

const ProfileAction = (props) => {
  const { auth, history, match, location } = props;
  const { userId } = match.params;
  const [profile, setProfile] = useState(props.profile);
  const [isLoaded, setLoaded] = useState(props.isLoaded);
  const isEdit = location.pathname.split("/").includes("edit");
  const userType = getUrlParam(location.search, "type", "user");

  useEffect(() => {
    if (isEdit) {
      if (userId === "@self") {
        setProfile(auth.user);
        setLoaded(true);
      } else {
        getUser(userId)
          .then((res) => {
            if (res.data.error)
              throw (res.data.error);

            setProfile(res.data.response);
            setLoaded(true);
          })
          .catch(error => {
            console.error(error);
          });
      }
    } else {
      setProfile(null);
      setLoaded(true);
    }
  }, [isEdit, auth.user, userId]);
  

  return (
    isLoaded ?
      <PageLayout
        articleHeaderContent={<ArticleHeaderContent />}
        articleBodyContent={<ArticleBodyContent />}
        sectionHeaderContent={<SectionHeaderContent profile={profile} history={history} userType={userType} />}
        sectionBodyContent={<SectionBodyContent profile={profile} userType={userType} />}
      />
    : <PageLayout
        sectionBodyContent={<Loader className="pageLoader" type="rombs" size={40} />}
      />
  );
};

ProfileAction.propTypes = {
  auth: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    auth: state.auth
  };
}

export default connect(mapStateToProps)(withRouter(ProfileAction));