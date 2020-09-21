import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { Backdrop, ProgressBar, utils } from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { ARTICLE_PINNED_KEY } from "../../constants";

import Article from "./sub-components/article";
import SubArticleHeader from "./sub-components/article-header";
import SubArticleMainButton from "./sub-components/article-main-button";
import SubArticleBody from "./sub-components/article-body";
import ArticlePinPanel from "./sub-components/article-pin-panel";
import Section from "./sub-components/section";
import SubSectionHeader from "./sub-components/section-header";
import SubSectionFilter from "./sub-components/section-filter";
import SubSectionBody from "./sub-components/section-body";
import SubSectionBodyContent from "./sub-components/section-body-content";
import SubSectionPaging from "./sub-components/section-paging";
import SectionToggler from "./sub-components/section-toggler";
import { changeLanguage } from "../../utils";
import ReactResizeDetector from "react-resize-detector";

const { size } = utils.device;

function ArticleHeader() {
  return null;
}
ArticleHeader.displayName = "ArticleHeader";

function ArticleMainButton() {
  return null;
}
ArticleMainButton.displayName = "ArticleMainButton";

function ArticleBody() {
  return null;
}
ArticleBody.displayName = "ArticleBody";

function SectionHeader() {
  return null;
}
SectionHeader.displayName = "SectionHeader";

function SectionFilter() {
  return null;
}
SectionFilter.displayName = "SectionFilter";

function SectionBody() {
  return null;
}
SectionBody.displayName = "SectionBody";

function SectionPaging() {
  return null;
}
SectionPaging.displayName = "SectionPaging";

class PageLayoutComponent extends React.Component {
  static ArticleHeader = ArticleHeader;
  static ArticleMainButton = ArticleMainButton;
  static ArticleBody = ArticleBody;
  static SectionHeader = SectionHeader;
  static SectionFilter = SectionFilter;
  static SectionBody = SectionBody;
  static SectionPaging = SectionPaging;

  constructor(props) {
    super(props);

    const isArticleVisibleAndPinned = !!localStorage.getItem(
      ARTICLE_PINNED_KEY
    );

    this.state = {
      isBackdropVisible: false,
      isArticleVisible: isArticleVisibleAndPinned,
      isArticlePinned: isArticleVisibleAndPinned
    };
  }

  componentDidMount() {
    window.addEventListener("orientationchange", this.orientationChangeHandler);
    this.orientationChangeHandler();
  }

  componentWillUnmount() {
    window.removeEventListener(
      "orientationchange",
      this.orientationChangeHandler
    );
  }

  orientationChangeHandler = () => {
    const isValueExist = !!localStorage.getItem(ARTICLE_PINNED_KEY);
    const isEnoughWidth = screen.availWidth > size.smallTablet;

    if (!isEnoughWidth && isValueExist) {
      this.backdropClick();
    }
    if (isEnoughWidth && isValueExist) {
      this.pinArticle();
    }
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

    localStorage.setItem(ARTICLE_PINNED_KEY, true);
  };

  unpinArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticlePinned: false,
      isArticleVisible: true
    });

    localStorage.removeItem(ARTICLE_PINNED_KEY);
  };

  showArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  onResize = (width, height) => {
    //console.log(`onResize height: ${height}, width: ${width}`);
  };

  render() {
    const {
      onDrop,
      progressBarDropDownContent,
      progressBarLabel,
      progressBarValue,
      setSelections,
      showProgressBar,
      uploadFiles,
      viewAs,
      withBodyAutoFocus,
      withBodyScroll,
      children
    } = this.props;

    let articleHeaderContent = null;
    let articleMainButtonContent = null;
    let articleBodyContent = null;
    let sectionHeaderContent = null;
    let sectionFilterContent = null;
    let sectionPagingContent = null;
    let sectionBodyContent = null;

    React.Children.forEach(children, child => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case ArticleHeader.displayName:
          articleHeaderContent = child;
          break;
        case ArticleMainButton.displayName:
          articleMainButtonContent = child;
          break;
        case ArticleBody.displayName:
          articleBodyContent = child;
          break;
        case SectionHeader.displayName:
          sectionHeaderContent = child;
          break;
        case SectionFilter.displayName:
          sectionFilterContent = child;
          break;
        case SectionPaging.displayName:
          sectionPagingContent = child;
          break;
        case SectionBody.displayName:
          sectionBodyContent = child;
          break;
        default:
          break;
      }
    });

    const isArticleHeaderAvailable = !!articleHeaderContent,
      isArticleMainButtonAvailable = !!articleMainButtonContent,
      isArticleBodyAvailable = !!articleBodyContent,
      isArticleAvailable =
        isArticleHeaderAvailable ||
        isArticleMainButtonAvailable ||
        isArticleBodyAvailable,
      isSectionHeaderAvailable = !!sectionHeaderContent,
      isSectionFilterAvailable = !!sectionFilterContent,
      isSectionPagingAvailable = !!sectionPagingContent,
      isSectionBodyAvailable =
        !!sectionBodyContent ||
        isSectionFilterAvailable ||
        isSectionPagingAvailable,
      isSectionAvailable =
        isSectionHeaderAvailable ||
        isSectionFilterAvailable ||
        isSectionBodyAvailable ||
        isSectionPagingAvailable ||
        isArticleAvailable,
      isBackdropAvailable = isArticleAvailable;

    return (
      <>
        {isBackdropAvailable && (
          <Backdrop
            zIndex={400}
            visible={this.state.isBackdropVisible}
            onClick={this.backdropClick}
          />
        )}
        {isArticleAvailable && (
          <Article
            visible={this.state.isArticleVisible}
            pinned={this.state.isArticlePinned}
          >
            {isArticleHeaderAvailable && (
              <SubArticleHeader>
                {articleHeaderContent
                  ? articleHeaderContent.props.children
                  : null}
              </SubArticleHeader>
            )}
            {isArticleMainButtonAvailable && (
              <SubArticleMainButton>
                {articleMainButtonContent
                  ? articleMainButtonContent.props.children
                  : null}
              </SubArticleMainButton>
            )}
            {isArticleBodyAvailable && (
              <SubArticleBody>
                {articleBodyContent ? articleBodyContent.props.children : null}
              </SubArticleBody>
            )}
            {isArticleBodyAvailable && (
              <ArticlePinPanel
                pinned={this.state.isArticlePinned}
                pinText={this.props.t("Pin")}
                onPin={this.pinArticle}
                unpinText={this.props.t("Unpin")}
                onUnpin={this.unpinArticle}
              />
            )}
          </Article>
        )}
        {isSectionAvailable && (
          <ReactResizeDetector
            onResize={this.onResize}
            refreshRate={200}
            refreshMode="debounce"
          >
            {({ width }) => (
              <Section widthProp={width}>
                {isSectionHeaderAvailable && (
                  <SubSectionHeader
                    isArticlePinned={this.state.isArticlePinned}
                  >
                    {sectionHeaderContent
                      ? sectionHeaderContent.props.children
                      : null}
                  </SubSectionHeader>
                )}
                {isSectionFilterAvailable && (
                  <SubSectionFilter className="section-header_filter">
                    {sectionFilterContent
                      ? sectionFilterContent.props.children
                      : null}
                  </SubSectionFilter>
                )}
                {isSectionBodyAvailable && (
                  <>
                    <SubSectionBody
                      onDrop={onDrop}
                      uploadFiles={uploadFiles}
                      setSelections={setSelections}
                      withScroll={withBodyScroll}
                      autoFocus={withBodyAutoFocus}
                      pinned={this.state.isArticlePinned}
                      viewAs={viewAs}
                    >
                      {isSectionFilterAvailable && (
                        <SubSectionFilter className="section-body_filter">
                          {sectionFilterContent
                            ? sectionFilterContent.props.children
                            : null}
                        </SubSectionFilter>
                      )}
                      <SubSectionBodyContent>
                        {sectionBodyContent
                          ? React.cloneElement(
                              sectionBodyContent.props.children,
                              { widthProp: width }
                            )
                          : null}
                      </SubSectionBodyContent>
                      {isSectionPagingAvailable && (
                        <SubSectionPaging>
                          {sectionPagingContent
                            ? sectionPagingContent.props.children
                            : null}
                        </SubSectionPaging>
                      )}
                    </SubSectionBody>
                    {showProgressBar && (
                      <ProgressBar
                        className="layout-progress-bar"
                        label={progressBarLabel}
                        percent={progressBarValue}
                        dropDownContent={progressBarDropDownContent}
                      />
                    )}
                  </>
                )}

                {isArticleAvailable && (
                  <SectionToggler
                    visible={!this.state.isArticleVisible}
                    onClick={this.showArticle}
                  />
                )}
              </Section>
            )}
          </ReactResizeDetector>
        )}
      </>
    );
  }
}

PageLayoutComponent.propTypes = {
  children: PropTypes.any,
  withBodyScroll: PropTypes.bool,
  withBodyAutoFocus: PropTypes.bool,
  t: PropTypes.func,

  showProgressBar: PropTypes.bool,
  progressBarValue: PropTypes.number,
  progressBarDropDownContent: PropTypes.any,
  progressBarLabel: PropTypes.string,
  onDrop: PropTypes.func,
  setSelections: PropTypes.func,
  uploadFiles: PropTypes.bool
};

PageLayoutComponent.defaultProps = {
  withBodyScroll: true,
  withBodyAutoFocus: false
};

const PageLayoutTranslated = withTranslation()(PageLayoutComponent);

const PageLayout = props => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <PageLayoutTranslated i18n={i18n} {...props} />;
};

PageLayout.ArticleHeader = ArticleHeader;
PageLayout.ArticleMainButton = ArticleMainButton;
PageLayout.ArticleBody = ArticleBody;
PageLayout.SectionHeader = SectionHeader;
PageLayout.SectionFilter = SectionFilter;
PageLayout.SectionBody = SectionBody;
PageLayout.SectionPaging = SectionPaging;

PageLayout.propTypes = {
  language: PropTypes.string,
  children: PropTypes.any
};

export default PageLayout;
