import React, { useEffect, useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { getUser } from '../../../utils/api';

const ProfileAction = (props) => {
  const { auth, history, match, location } = props;
  const { userId } = match.params;
  const [profile, setProfile] = useState(props.profile);
  const [isLoaded, setLoaded] = useState(props.isLoaded);
  const isEdit = location.pathname.split('/').includes('edit');

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
  }, []);
  

  return (
    isLoaded ?
      <PageLayout
        articleHeaderContent={<ArticleHeaderContent />}
        articleBodyContent={<ArticleBodyContent />}
        sectionHeaderContent={<SectionHeaderContent profile={profile} history={history} />}
        sectionBodyContent={<SectionBodyContent profile={profile} />}
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