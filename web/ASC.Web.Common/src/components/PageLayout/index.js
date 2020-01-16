import React from "react";
import PropTypes from "prop-types";
import { Backdrop } from "asc-web-components";
import { withTranslation } from 'react-i18next';
import i18n from './i18n';
import { connect } from "react-redux";

import Article from "./sub-components/article";
import ArticleHeader from "./sub-components/article-header";
import ArticleMainButton from "./sub-components/article-main-button";
import ArticleBody from "./sub-components/article-body";
import ArticlePinPanel from "./sub-components/article-pin-panel";
import Section from "./sub-components/section";
import SectionHeader from "./sub-components/section-header";
import SectionFilter from "./sub-components/section-filter";
import SectionBody from "./sub-components/section-body";
import SectionPaging from "./sub-components/section-paging";
import SectionToggler from "./sub-components/section-toggler";

class PageLayoutComponent extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);
  }

  componentDidUpdate(prevProps) {
    if (this.hasChanges(this.props, prevProps)) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  hasChanges = (currentProps, prevProps) => {
    return (
      currentProps.articleHeaderContent != prevProps.articleHeaderContent ||
      currentProps.articleMainButtonContent != prevProps.articleMainButtonContent ||
      currentProps.articleBodyContent != prevProps.articleBodyContent ||
      currentProps.sectionHeaderContent != prevProps.sectionHeaderContent ||
      currentProps.sectionFilterContent != prevProps.sectionFilterContent ||
      currentProps.sectionBodyContent != prevProps.sectionBodyContent ||
      currentProps.sectionPagingContent != prevProps.sectionPagingContent
    );
  };

  mapPropsToState = props => {
    let isArticleHeaderAvailable = !!props.articleHeaderContent,
      isArticleMainButtonAvailable = !!props.articleMainButtonContent,
      isArticleBodyAvailable = !!props.articleBodyContent,
      isArticleAvailable = isArticleHeaderAvailable || isArticleMainButtonAvailable || isArticleBodyAvailable,
      isSectionHeaderAvailable = !!props.sectionHeaderContent,
      isSectionFilterAvailable = !!props.sectionFilterContent,
      isSectionPagingAvailable = !!props.sectionPagingContent,
      isSectionBodyAvailable = !!props.sectionBodyContent || isSectionFilterAvailable || isSectionPagingAvailable,
      isSectionAvailable = isSectionHeaderAvailable || isSectionFilterAvailable || isSectionBodyAvailable || isSectionPagingAvailable || isArticleAvailable,
      isBackdropAvailable = isArticleAvailable;

    let newState = {
      isBackdropAvailable: isBackdropAvailable,
      isArticleAvailable: isArticleAvailable,
      isArticleHeaderAvailable: isArticleHeaderAvailable,
      isArticleMainButtonAvailable: isArticleMainButtonAvailable,
      isArticleBodyAvailable: isArticleBodyAvailable,
      isSectionAvailable: isSectionAvailable,
      isSectionHeaderAvailable: isSectionHeaderAvailable,
      isSectionFilterAvailable: isSectionFilterAvailable,
      isSectionBodyAvailable: isSectionBodyAvailable,
      isSectionPagingAvailable: isSectionPagingAvailable,

      isBackdropVisible: props.isBackdropVisible,
      isArticleVisible: props.isArticleVisible,
      isArticlePinned: props.isArticlePinned,

      articleHeaderContent: props.articleHeaderContent,
      articleMainButtonContent: props.articleMainButtonContent,
      articleBodyContent: props.articleBodyContent,
      sectionHeaderContent: props.sectionHeaderContent,
      sectionFilterContent: props.sectionFilterContent,
      sectionBodyContent: props.sectionBodyContent,
      sectionPagingContent: props.sectionPagingContent
    };

    return newState;
  };

  backdropClick = () => {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    });
  };

  pinArticle = () => {
    this.setState({
      isBackdropVisible: false,
      isArticlePinned: true,
      isArticleVisible: true
    });
  };

  unpinArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticlePinned: false,
      isArticleVisible: true
    });
  };

  showArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  render() {
    return (
      <>
        {this.state.isBackdropAvailable && (
          <Backdrop
            zIndex={400}
            visible={this.state.isBackdropVisible}
            onClick={this.backdropClick}
          />
        )}
        {this.state.isArticleAvailable && (
          <Article
            visible={this.state.isArticleVisible}
            pinned={this.state.isArticlePinned}
          >
            {this.state.isArticleHeaderAvailable && (
              <ArticleHeader>
                {this.state.articleHeaderContent}
              </ArticleHeader>
            )}
            {this.state.isArticleMainButtonAvailable && (
              <ArticleMainButton>
                {this.state.articleMainButtonContent}
              </ArticleMainButton>
            )}
            {this.state.isArticleBodyAvailable && (
              <ArticleBody>{this.state.articleBodyContent}</ArticleBody>
            )}
            {this.state.isArticleBodyAvailable && (
              <ArticlePinPanel
                pinned={this.state.isArticlePinned}
                pinText={this.props.t('Pin')}
                onPin={this.pinArticle}
                unpinText={this.props.t('Unpin')}
                onUnpin={this.unpinArticle}
              />
            )}
          </Article>
        )}
        {this.state.isSectionAvailable && (
          <Section>
            {this.state.isSectionHeaderAvailable && (
              <SectionHeader isArticlePinned={this.state.isArticlePinned}>{this.state.sectionHeaderContent}</SectionHeader>
            )}

            {this.state.isSectionBodyAvailable && (
              <SectionBody withScroll={this.props.withBodyScroll} pinned={this.state.isArticlePinned}>
                {this.state.isSectionFilterAvailable && (
                  <SectionFilter>{this.state.sectionFilterContent}</SectionFilter>
                )}
                {this.state.sectionBodyContent}
                {this.state.isSectionPagingAvailable && (
                  <SectionPaging>{this.state.sectionPagingContent}</SectionPaging>
                )}
              </SectionBody>
            )}

            {this.state.isArticleAvailable && (
              <SectionToggler
                visible={!this.state.isArticlePinned}
                onClick={this.showArticle}
              />
            )}
          </Section>
        )}
      </>
    );
  }
}

const PageLayoutTranslated = withTranslation()(PageLayoutComponent);
const Pagelayout = props => {
  const { language } = props;
  i18n.changeLanguage(language);

  return <PageLayoutTranslated i18n={i18n} {...props} />
}

Pagelayout.propTypes = {
  language:PropTypes.string,
}

PageLayoutComponent.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isArticleVisible: PropTypes.bool,
  isArticlePinned: PropTypes.bool,

  articleHeaderContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  articleMainButtonContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  articleBodyContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  sectionHeaderContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  sectionFilterContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  sectionBodyContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  sectionPagingContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),

  withBodyScroll: PropTypes.bool,
  t: PropTypes.func,
};

PageLayoutComponent.defaultProps = {
  isBackdropVisible: false,
  isArticleVisible: false,
  isArticlePinned: false,
  withBodyScroll: true
};

function mapStateToProps(state) {
  return {
    language:
      state.auth &&
      ((state.auth.user && state.auth.user.cultureName) ||
        (state.auth.settings && state.auth.settings.culture))
  };
}

export default connect(mapStateToProps)(Pagelayout);
