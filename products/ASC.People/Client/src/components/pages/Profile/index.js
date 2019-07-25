import React, { useEffect, useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import _ from "lodash";
import { Backdrop, NewPageLayout as NPL, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { getUser } from '../../../utils/api';

const Profile = (props) => {
  console.log("Profile render");
  
  const { auth, match } = props;
  const { userId } = match.params;

  const [profile, setProfile] = useState(props.profile);
  const [isLoaded, setLoaded] = useState(props.isLoaded);

  const [isBackdropVisible, setIsBackdropVisible] = useState(false);
  const [isArticleVisible, setIsArticleVisible] = useState(false);
  const [isArticlePinned, setIsArticlePinned] = useState(false);

  useEffect(() => {
    if (userId === "@self" || userId === auth.user.userName) {
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
  }, [auth.user, userId]);

  const onBackdropClick = () => {
    setIsBackdropVisible(false);
    setIsArticleVisible(false);
    setIsArticlePinned(false);
  };

  const onPinArticle = () => {
    setIsBackdropVisible(false);
    setIsArticleVisible(true);
    setIsArticlePinned(true);
  };

  const onUnpinArticle = () => {
    setIsBackdropVisible(true);
    setIsArticleVisible(true);
    setIsArticlePinned(false);
  };

  const onShowArticle = () => {
    setIsBackdropVisible(true);
    setIsArticleVisible(true);
    setIsArticlePinned(false);
  };

  return (
    isLoaded
      ? <>
        <Backdrop visible={isBackdropVisible} onClick={onBackdropClick} />
        <NPL.Article visible={isArticleVisible} pinned={isArticlePinned}>
          <NPL.ArticleHeader visible={isArticlePinned}>
            <ArticleHeaderContent />
          </NPL.ArticleHeader>
          <NPL.ArticleMainButton>
            <ArticleMainButtonContent />
          </NPL.ArticleMainButton>
          <NPL.ArticleBody>
            <ArticleBodyContent />
          </NPL.ArticleBody>
          <NPL.ArticlePinPanel pinned={isArticlePinned} pinText="Pin this panel" onPin={onPinArticle} unpinText="Unpin this panel" onUnpin={onUnpinArticle} />
        </NPL.Article>
        <NPL.Section>
          <NPL.SectionHeader>
            <SectionHeaderContent profile={profile} />
          </NPL.SectionHeader>
          <NPL.SectionBody>
            <SectionBodyContent profile={profile} isAdmin={auth.user.isAdmin || auth.user.isOwner} isSelf={userId === "@self" || userId === auth.user.userName} />
          </NPL.SectionBody>
          <NPL.SectionToggler visible={!isArticlePinned} onClick={onShowArticle} />
        </NPL.Section>
      </>
      : <>
        <NPL.Section>
          <NPL.SectionBody>
            <Loader className="pageLoader" type="rombs" size={40} />
          </NPL.SectionBody>
        </NPL.Section>
      </>
  );
};

Profile.propTypes = {
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

export default connect(mapStateToProps)(Profile);