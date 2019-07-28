import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Backdrop, NewPageLayout as NPL, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { setProfile, fetchProfile, resetProfile } from '../../../store/profile/actions';
import { isMe } from '../../../store/auth/selectors';
import { getUserByUserName } from '../../../store/people/selectors';

class ProfileAction extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    }

    this.onBackdropClick = this.onBackdropClick.bind(this);
    this.onPinArticle = this.onPinArticle.bind(this);
    this.onUnpinArticle = this.onUnpinArticle.bind(this);
    this.onShowArticle = this.onShowArticle.bind(this);
  }

  onBackdropClick() {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    });
  }

  onPinArticle() {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: true,
      isArticlePinned: true
    });
  }

  onUnpinArticle() {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  }

  onShowArticle() {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  }

  componentDidMount() {
    const { auth, users, match, setProfile, fetchProfile } = this.props;
    const { userId, type } = match.params;

    if (!userId) {
      setProfile({ isVisitor: type === "guest" });
    }
    else if (isMe(auth, userId)) {
      setProfile(auth.user);
    } else {
      const user = getUserByUserName(users, userId);
      if (!user)
        fetchProfile(userId);
      else
        setProfile(user);
    }
  }

  componentDidUpdate(prevProps) {
    const { auth, users, match, setProfile, fetchProfile } = this.props;
    const { userId, type } = match.params;
    const prevUserId = prevProps.match.params.userId;
    const prevType = prevProps.match.params.type;

    if (!userId && type !== prevType) {
      setProfile({ isVisitor: type === "guest" });
    }
    else if (userId !== prevUserId) {
      if (isMe(auth, userId)) {
        setProfile(auth.user);
      } else {
        const user = getUserByUserName(users, userId);
        if (!user)
          fetchProfile(userId);
        else
          setProfile(user);
      }
    }
  }

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    console.log("ProfileAction render")

    const { isBackdropVisible, isArticleVisible, isArticlePinned } = this.state;
    const { profile, match } = this.props;
    const { type } = match.params;

    return (
      profile
        ? <>
          <Backdrop visible={isBackdropVisible} onClick={this.onBackdropClick} />
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
            <NPL.ArticlePinPanel pinned={isArticlePinned} pinText="Pin this panel" onPin={this.onPinArticle} unpinText="Unpin this panel" onUnpin={this.onUnpinArticle} />
          </NPL.Article>
          <NPL.Section>
            <NPL.SectionHeader>
              <SectionHeaderContent profile={profile} userType={type} />
            </NPL.SectionHeader>
            <NPL.SectionBody>
              <SectionBodyContent profile={profile} userType={type} />
            </NPL.SectionBody>
            <NPL.SectionToggler visible={!isArticlePinned} onClick={this.onShowArticle} />
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
  }
}

ProfileAction.propTypes = {
  auth: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  fetchProfile: PropTypes.func.isRequired,
  setProfile: PropTypes.func.isRequired
};

function mapStateToProps(state) {
  return {
    auth: state.auth,
    profile: state.profile.targetUser,
    users: state.people.users
  };
}

export default connect(mapStateToProps, {
  setProfile,
  fetchProfile,
  resetProfile
})(ProfileAction);