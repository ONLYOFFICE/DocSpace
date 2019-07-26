import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Backdrop, NewPageLayout as NPL, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { setUser, fetchAndSetUser } from '../../../actions/peopleActions';


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
    const { auth, match } = this.props;
    const { userId, type } = match.params;
    if (userId) {
      if (userId === "@self") {
        this.props.setUser(auth.user);
      } else {
        this.props.fetchAndSetUser(userId);
      }
    } else {
      this.props.setUser({isVisitor: type === "guest"});
    } 
  }

  componentDidUpdate(prevProps) {
    const currentParams = this.props.match.params;
    const prevParams = prevProps.match.params;

    if (currentParams.userId !== prevParams.userId || currentParams.type !== prevParams.type) {
      if (currentParams.userId) {
        if (currentParams.userId === "@self") {
          this.props.setUser(this.props.auth.user);
        } else {
          this.props.fetchAndSetUser(currentParams.userId);
        }
      } else {
        this.props.setUser({isVisitor: currentParams.type === "guest"});
      }
    }
  }

  componentWillUnmount() {
    this.props.setUser(null);
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
  fetchAndSetUser: PropTypes.func.isRequired,
  setUser: PropTypes.func.isRequired
};

function mapStateToProps(state) {
  return {
    auth: state.auth,
    profile: state.people.targetUser
  };
}

function mapDispatchToProps(dispatch){
  return {
    fetchAndSetUser: function (userId) {
      dispatch(fetchAndSetUser(userId));
    },
    setUser: function (user){
      dispatch(setUser(user));
    }
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(ProfileAction);