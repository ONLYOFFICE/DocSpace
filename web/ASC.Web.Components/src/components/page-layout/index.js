import React from 'react'
import PropTypes from 'prop-types'
import Backdrop from '../backdrop'

import Article from './sub-comoponents/article'
import ArticleHeader from './sub-comoponents/article-header'
import ArticleMainButton from './sub-comoponents/article-main-button'
import ArticleBody from './sub-comoponents/article-body'
import ArticlePinPanel from './sub-comoponents/article-pin-panel'
import Section from './sub-comoponents/section'
import SectionHeader from './sub-comoponents/section-header'
import SectionFilter from './sub-comoponents/section-filter'
import SectionBody from './sub-comoponents/section-body'
import SectionPaging from './sub-comoponents/section-paging'
import SectionToggler from './sub-comoponents/section-toggler'

class PageLayout extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.hasChanges(this.props,prevProps)) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  hasChanges = (currentProps, prevProps) => {
    return currentProps.articleHeaderContent != prevProps.articleHeaderContent ||
    currentProps.articleMainButtonContent != prevProps.articleMainButtonContent ||
    currentProps.articleBodyContent != prevProps.articleBodyContent ||
    currentProps.sectionHeaderContent != prevProps.sectionHeaderContent ||
    currentProps.sectionFilterContent != prevProps.sectionFilterContent ||
    currentProps.sectionBodyContent != prevProps.sectionBodyContent ||
    currentProps.sectionPagingContent != prevProps.sectionPagingContent;
  }

  mapPropsToState = (props) => {
    let isArticleHeaderAvailable = !!props.articleHeaderContent,
        isArticleMainButtonAvailable = !!props.articleMainButtonContent,
        isArticleBodyAvailable = !!props.articleBodyContent,
        isArticleAvailable = isArticleHeaderAvailable || isArticleMainButtonAvailable || isArticleBodyAvailable,
        isSectionHeaderAvailable = !!props.sectionHeaderContent,
        isSectionFilterAvailable = !!props.sectionFilterContent,
        isSectionBodyAvailable = !!props.sectionBodyContent,
        isSectionPagingAvailable = !!props.sectionPagingContent,
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
      sectionPagingContent: props.sectionPagingContent,
    };

    return newState;
  }

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
        {
          this.state.isBackdropAvailable &&
          <Backdrop zIndex={400} visible={this.state.isBackdropVisible} onClick={this.backdropClick}/>
        }
        {
          this.state.isArticleAvailable &&
          <Article visible={this.state.isArticleVisible} pinned={this.state.isArticlePinned}>
            {
              this.state.isArticleHeaderAvailable &&
              <ArticleHeader visible={this.state.isArticlePinned}>{this.state.articleHeaderContent}</ArticleHeader>
            }
            {
              this.state.isArticleMainButtonAvailable &&
              <ArticleMainButton>{this.state.articleMainButtonContent}</ArticleMainButton>
            }
            {
              this.state.isArticleBodyAvailable &&
              <ArticleBody>{this.state.articleBodyContent}</ArticleBody>
            }
            {
              this.state.isArticleBodyAvailable &&
              <ArticlePinPanel pinned={this.state.isArticlePinned} pinText="Pin this panel" onPin={this.pinArticle} unpinText="Unpin this panel" onUnpin={this.unpinArticle}/>
            }
          </Article>
        }
        {
          this.state.isSectionAvailable &&
          <Section>
            {
              this.state.isSectionHeaderAvailable &&
              <SectionHeader>{this.state.sectionHeaderContent}</SectionHeader>
            }
            {
              this.state.isSectionFilterAvailable &&
              <SectionFilter>{this.state.sectionFilterContent}</SectionFilter>
            }
            {
              this.state.isSectionBodyAvailable &&
              <SectionBody withScroll={this.props.withBodyScroll}>{this.state.sectionBodyContent}</SectionBody>
            }
            {
              this.state.isSectionPagingAvailable &&
              <SectionPaging>{this.state.sectionPagingContent}</SectionPaging>
            }
            {
              this.state.isArticleAvailable &&
              <SectionToggler visible={!this.state.isArticlePinned} onClick={this.showArticle}/>
            }
          </Section>
        }
      </>
    )
  }
}

PageLayout.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isArticleVisible: PropTypes.bool,
  isArticlePinned: PropTypes.bool,

  articleHeaderContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  articleMainButtonContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  articleBodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionHeaderContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionFilterContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionBodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionPagingContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),

  withBodyScroll:  PropTypes.bool
}

PageLayout.defaultProps = {
  isBackdropVisible: false,
  isArticleVisible: false,
  isArticlePinned: false,
  withBodyScroll: true
}

export default PageLayout