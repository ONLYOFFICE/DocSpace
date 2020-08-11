import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from "./i18n";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent
} from "./Section";
import { setSelected } from "../../../store/people/actions";
import { getSelectedGroup } from "../../../store/people/selectors";
const { changeLanguage } = utils;

class PureHome extends React.Component {
  constructor(props) {
    super(props);

    const currentGroup = getSelectedGroup(props.groups, props.selectedGroup);
    document.title = currentGroup
      ? `${currentGroup.name} – ${props.t("People")}`
      : `${props.t("People")} – ${props.t("OrganizationName")}`;

    this.state = {
      isHeaderVisible: false,
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
      isLoading: false
    };
  }

  renderGroupButtonMenu = () => {
    const { users, selection, selected, setSelected } = this.props;

    const headerVisible = selection.length > 0;
    const headerIndeterminate =
      headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;

    let newState = {};

    if (headerVisible || selected === "close") {
      newState.isHeaderVisible = headerVisible;
      if (selected === "close") {
        setSelected("none");
      }
    }

    newState.isHeaderIndeterminate = headerIndeterminate;
    newState.isHeaderChecked = headerChecked;

    this.setState(newState);
  };

  componentDidUpdate(prevProps) {
    if (this.props.selection !== prevProps.selection) {
      this.renderGroupButtonMenu();
    }
  }

  onSectionHeaderContentCheck = checked => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSectionHeaderContentSelect = selected => {
    this.props.setSelected(selected);
  };

  onClose = () => {
    const { setSelected } = this.props;
    setSelected("none");
    this.setState({ isHeaderVisible: false });
  };

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  render() {
    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      selected
    } = this.state;
    const { t } = this.props;
    return (
      <>
        <RequestLoader
          visible={this.state.isLoading}
          zIndex={256}
          loaderSize='16px'
          loaderColor={"#999"}
          label={`${t('LoadingProcessing')} ${t('LoadingDescription')}`}
          fontSize='12px'
          fontColor={"#999"}
        />
        <PageLayout
          withBodyScroll={true}
          withBodyAutoFocus={true}
        >
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>

          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>

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
              selected={selected}
              onLoading={this.onLoading}
              onChange={this.onRowChange}
            />
          </PageLayout.SectionBody>
          
          <PageLayout.SectionPaging>
            <SectionPagingContent onLoading={this.onLoading} />
          </PageLayout.SectionPaging>
        </PageLayout>
      </>
    );
  }
}

const HomeContainer = withTranslation()(PureHome);

const Home = props => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><HomeContainer {...props} /></I18nextProvider>);
}

Home.propTypes = {
  users: PropTypes.array,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    users: state.people.users,
    selection: state.people.selection,
    selected: state.people.selected,
    isLoaded: state.auth.isLoaded,
    selectedGroup: state.people.selectedGroup,
    groups: state.people.groups
  };
}

export default connect(
  mapStateToProps,
  { setSelected }
)(withRouter(Home));
