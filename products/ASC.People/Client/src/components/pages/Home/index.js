import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
//import { RequestLoader } from "asc-web-components";
import { PageLayout, utils, store } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
} from "./Section";
import { setSelected, setIsLoading } from "../../../store/people/actions";
import { createI18N } from "../../../helpers/i18n";
import { isMobile } from "react-device-detect";
import { getIsLoading } from "../../../store/people/selectors";
const i18n = createI18N({
  page: "Home",
  localesPath: "pages/Home",
});
const { changeLanguage } = utils;
const {
  isAdmin,
  getIsLoaded,
  getOrganizationName,
  getHeaderVisible,
} = store.auth.selectors;
const { setHeaderVisible } = store.auth.actions;
class PureHome extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
    };
    this.isCancelScrollUp = false;
    this.isCloseContextMenu = false;
  }

  renderGroupButtonMenu = () => {
    const {
      users,
      selection,
      selected,
      setSelected,
      setHeaderVisible,
    } = this.props;

    const headerVisible = selection.length > 0;
    const headerIndeterminate =
      headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;

    let newState = {};

    if (headerVisible || selected === "close") {
      setHeaderVisible(headerVisible);
      if (selected === "close") {
        this.isCancelScrollUp = true;
        this.isCloseContextMenu = true;
        setSelected("none");
      }
    }
    newState.isHeaderIndeterminate = headerIndeterminate;
    newState.isHeaderChecked = headerChecked;

    this.setState(newState);
  };

  componentDidMount() {
    console.log(" componentDidMount ");
    this.documentElement = document.getElementById("customScrollBar");
  }
  componentDidUpdate(prevProps) {
    if (this.props.selection !== prevProps.selection) {
      this.renderGroupButtonMenu();
    }

    if (this.props.isLoading !== prevProps.isLoading) {
      if (this.props.isLoading) {
        utils.showLoader();
      } else {
        utils.hideLoader();
      }

      if (this.isCancelScrollUp && !this.props.isLoading)
        this.isCancelScrollUp = false;

      if (this.isCancelScrollUp && this.isDeleteProfile)
        this.isCancelScrollUp = false;

      if (this.isCancelScrollUp && this.isCloseContextMenu) {
        this.isCancelScrollUp = false;
        this.isCloseContextMenu = false;
      }
    } else {
      !this.props.selection.length > 0 &&
        !this.isCancelScrollUp &&
        this.documentElement &&
        this.documentElement.scrollTo(0, 0);
    }
  }

  onSectionHeaderContentCheck = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSectionHeaderContentSelect = (selected) => {
    this.props.setSelected(selected);
  };

  onClose = () => {
    const { setSelected, setHeaderVisible } = this.props;
    this.isCancelScrollUp = true;
    this.isCloseContextMenu = true;
    setSelected("none");
    setHeaderVisible(false);
  };

  onLoading = (status) => {
    console.log("Set is loading", status);
    this.props.setIsLoading(status);
  };

  onCancelScrollUp = (value, isDeleteProfile) => {
    this.isCancelScrollUp = value;
    this.isDeleteProfile = isDeleteProfile;
  };
  render() {
    const { isHeaderIndeterminate, isHeaderChecked, selected } = this.state;

    const { isAdmin, isLoaded, isHeaderVisible } = this.props;
    console.log("Render Home at people", this.props);
    return (
      <PageLayout
        withBodyScroll={true}
        withBodyAutoFocus={!isMobile}
        isLoaded={isLoaded}
        isHeaderVisible={isHeaderVisible}
      >
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        {isAdmin && (
          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>
        )}

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent
            isHeaderVisible={isHeaderVisible}
            isHeaderIndeterminate={isHeaderIndeterminate}
            isHeaderChecked={isHeaderChecked}
            onCheck={this.onSectionHeaderContentCheck}
            onSelect={this.onSectionHeaderContentSelect}
            onClose={this.onClose}
            onLoading={this.onLoading}
          />
        </PageLayout.SectionHeader>

        <PageLayout.SectionFilter>
          <SectionFilterContent onLoading={this.onLoading} />
        </PageLayout.SectionFilter>

        <PageLayout.SectionBody>
          <SectionBodyContent
            isMobile={isMobile}
            selected={selected}
            onLoading={this.onLoading}
            onCancelScrollUp={this.onCancelScrollUp}
            onChange={this.onRowChange}
          />
        </PageLayout.SectionBody>

        <PageLayout.SectionPaging>
          <SectionPagingContent onLoading={this.onLoading} />
        </PageLayout.SectionPaging>
      </PageLayout>
    );
  }
}

const HomeContainer = withTranslation()(PureHome);

const Home = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <HomeContainer {...props} />
    </I18nextProvider>
  );
};

Home.propTypes = {
  users: PropTypes.array,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool,
  isAdmin: PropTypes.bool,
};

function mapStateToProps(state) {
  const { users, selection, selected, selectedGroup, groups } = state.people;
  return {
    users,
    selection,
    selected,
    selectedGroup,
    groups,
    organizationName: getOrganizationName(state),
    isAdmin: isAdmin(state),
    isLoading: getIsLoading(state),
    isLoaded: getIsLoaded(state),
    isHeaderVisible: getHeaderVisible(state),
  };
}

export default connect(mapStateToProps, {
  setSelected,
  setIsLoading,
  setHeaderVisible,
})(withRouter(Home));
