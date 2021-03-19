import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { Backdrop, utils } from "asc-web-components";
import store from "../../store";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import i18n from "./i18n";
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
import FloatingButton from "../FloatingButton";
import { inject, observer } from "mobx-react";

const { size } = utils.device;
const { Provider } = utils.context;

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

    const isArticleVisibleAndPinned = !!this.props.isArticlePinned;

    this.state = {
      isBackdropVisible: false,
      isArticleVisible: isArticleVisibleAndPinned,
      isArticlePinned: isArticleVisibleAndPinned,
    };

    this.timeoutHandler = null;
    this.intervalHandler = null;
  }

  componentDidUpdate(prevProps) {
    if (
      this.props.hideAside &&
      !this.state.isArticlePinned &&
      this.props.hideAside !== prevProps.hideAside
    ) {
      this.backdropClick();
    }
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

    if (this.intervalHandler) clearInterval(this.intervalHandler);
    if (this.timeoutHandler) clearTimeout(this.timeoutHandler);
  }

  orientationChangeHandler = () => {
    const isValueExist = !!this.props.isArticlePinned;
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
      isArticlePinned: false,
    });
  };

  pinArticle = () => {
    this.setState({
      isBackdropVisible: false,
      isArticlePinned: true,
      isArticleVisible: true,
    });

    this.props.setArticlePinned(true);
  };

  unpinArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticlePinned: false,
      isArticleVisible: true,
    });

    this.props.setArticlePinned(false);
  };

  showArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false,
    });
  };

  render() {
    const {
      onDrop,
      showPrimaryProgressBar,
      primaryProgressBarIcon,
      primaryProgressBarValue,
      showPrimaryButtonAlert,
      showSecondaryProgressBar,
      secondaryProgressBarValue,
      secondaryProgressBarIcon,
      showSecondaryButtonAlert,
      setSelections,
      uploadFiles,
      viewAs,
      withBodyAutoFocus,
      withBodyScroll,
      children,
      isLoaded,
      isHeaderVisible,
      headerBorderBottom,
      onOpenUploadPanel,
      isTabletView,
      firstLoad,
    } = this.props;

    let articleHeaderContent = null;
    let articleMainButtonContent = null;
    let articleBodyContent = null;
    let sectionHeaderContent = null;
    let sectionFilterContent = null;
    let sectionPagingContent = null;
    let sectionBodyContent = null;

    React.Children.forEach(children, (child) => {
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
            isLoaded={isLoaded}
            firstLoad={firstLoad}
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
            refreshRate={100}
            refreshMode="debounce"
            refreshOptions={{ trailing: true }}
          >
            {({ width, height }) => (
              <Provider
                value={{
                  sectionWidth: width,
                  sectionHeight: height,
                }}
              >
                <Section
                  widthProp={width}
                  unpinArticle={this.unpinArticle}
                  pinned={this.state.isArticlePinned}
                >
                  {isSectionHeaderAvailable && (
                    <SubSectionHeader
                      isHeaderVisible={isHeaderVisible}
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
                        autoFocus={isMobile || isTabletView ? false : true}
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
                            ? sectionBodyContent.props.children
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
                    </>
                  )}

                  {showPrimaryProgressBar && showSecondaryProgressBar ? (
                    <>
                      <FloatingButton
                        className="layout-progress-bar"
                        icon={primaryProgressBarIcon}
                        percent={primaryProgressBarValue}
                        alert={showPrimaryButtonAlert}
                        onClick={onOpenUploadPanel}
                      />
                      <FloatingButton
                        className="layout-progress-second-bar"
                        icon={secondaryProgressBarIcon}
                        percent={secondaryProgressBarValue}
                        alert={showSecondaryButtonAlert}
                      />
                    </>
                  ) : showPrimaryProgressBar && !showSecondaryProgressBar ? (
                    <FloatingButton
                      className="layout-progress-bar"
                      icon={primaryProgressBarIcon}
                      percent={primaryProgressBarValue}
                      alert={showPrimaryButtonAlert}
                      onClick={onOpenUploadPanel}
                    />
                  ) : !showPrimaryProgressBar && showSecondaryProgressBar ? (
                    <FloatingButton
                      className="layout-progress-bar"
                      icon={secondaryProgressBarIcon}
                      percent={secondaryProgressBarValue}
                      alert={showSecondaryButtonAlert}
                    />
                  ) : (
                    <></>
                  )}

                  {isArticleAvailable && (
                    <SectionToggler
                      visible={!this.state.isArticleVisible}
                      onClick={this.showArticle}
                    />
                  )}
                </Section>
              </Provider>
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
  showPrimaryProgressBar: PropTypes.bool,
  primaryProgressBarValue: PropTypes.number,
  showPrimaryButtonAlert: PropTypes.bool,
  progressBarDropDownContent: PropTypes.any,
  primaryProgressBarIcon: PropTypes.string,
  showSecondaryProgressBar: PropTypes.bool,
  secondaryProgressBarValue: PropTypes.number,
  secondaryProgressBarIcon: PropTypes.string,
  showSecondaryButtonAlert: PropTypes.bool,
  onDrop: PropTypes.func,
  setSelections: PropTypes.func,
  uploadFiles: PropTypes.bool,
  hideAside: PropTypes.bool,
  isLoaded: PropTypes.bool,
  viewAs: PropTypes.string,
  uploadPanelVisible: PropTypes.bool,
  onOpenUploadPanel: PropTypes.func,
  isTabletView: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
  firstLoad: PropTypes.bool,
};

PageLayoutComponent.defaultProps = {
  withBodyScroll: true,
  withBodyAutoFocus: false,
};

const PageLayoutTranslated = withTranslation()(PageLayoutComponent);

const PageLayout = ({ language, ...rest }) => {
  useEffect(() => {
    changeLanguage(i18n, language);
  }, [language]);

  return <PageLayoutTranslated i18n={i18n} {...rest} />;
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
  children: PropTypes.any,
};

export default inject(({ auth }) => {
  const { language, settingsStore } = auth;
  const { isTabletView, isArticlePinned, setArticlePinned } = settingsStore;
  return {
    language,
    isTabletView,
    isArticlePinned,
    setArticlePinned,
  };
})(observer(PageLayout));
