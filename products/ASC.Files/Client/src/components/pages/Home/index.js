import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader, Checkbox, toastr } from "asc-web-components";
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
import { setSelected } from "../../../store/files/actions";
const { changeLanguage } = utils;

class PureHome extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isHeaderVisible: false,
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
      isLoading: false,

      showProgressBar: false,
      progressBarValue: 0,
      progressBarLabel: "",
      overwriteSetting: false,
      uploadOriginalFormatSetting: false,
      hideWindowSetting: false
    };
  }

  renderGroupButtonMenu = () => {
    const { files, selection, selected, setSelected, folders } = this.props;

    const headerVisible = selection.length > 0;
    const headerIndeterminate =
      headerVisible && selection.length > 0 && selection.length < files.length + folders.length;
    const headerChecked = headerVisible && selection.length === files.length + folders.length;

    /*console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      files.length=${files.length}
      selected=${selected}`);*/

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
    const { selection, setSelected } = this.props;

    if (!selection.length) {
      setSelected("none");
      this.setState({ isHeaderVisible: false });
    } else {
      setSelected("close");
    }
  };

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  setProgressVisible = (visible, timeout) => {
    const newTimeout = timeout ? timeout : 10000;
    if(visible) {this.setState({ showProgressBar: visible })}
    else { setTimeout(() => this.setState({ showProgressBar: visible, progressBarValue: 0 }), newTimeout)};
  };
  setProgressValue = value => this.setState({ progressBarValue: value });
  setProgressLabel = label => this.setState({ progressBarLabel: label });

  onChangeOverwrite = () => this.setState({overwriteSetting: !this.state.overwriteSetting})
  onChangeOriginalFormat = () => this.setState({uploadOriginalFormatSetting: !this.state.uploadOriginalFormatSetting})
  onChangeWindowVisible = () => this.setState({hideWindowSetting: !this.state.hideWindowSetting})

  startFilesOperations = progressBarLabel => {
    this.setState({ isLoading: true, progressBarLabel, showProgressBar: true })
  }

  finishFilesOperations = err => {
    const timeout = err ? 0 : null;
    err && toastr.error(err);
    this.onLoading(false);
    this.setProgressVisible(false, timeout);
  }

  render() {
    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      selected,
      isLoading,
      showProgressBar,
      progressBarValue,
      progressBarLabel,
      overwriteSetting,
      uploadOriginalFormatSetting,
      hideWindowSetting
    } = this.state;
    const { t } = this.props;

    const progressBarContent = (
      <div>
        <Checkbox
          onChange={this.onChangeOverwrite}
          isChecked={overwriteSetting}
          label={t("OverwriteSetting")}
        />
        <Checkbox
          onChange={this.onChangeOriginalFormat}
          isChecked={uploadOriginalFormatSetting}
          label={t("UploadOriginalFormatSetting")}
        />
        <Checkbox
          onChange={this.onChangeWindowVisible}
          isChecked={hideWindowSetting}
          label={t("HideWindowSetting")}
        />
      </div>
    );

    return (
      <>
        <RequestLoader
          visible={isLoading}
          zIndex={256}
          loaderSize='16px'
          loaderColor={"#999"}
          label={`${t('LoadingProcessing')} ${t('LoadingDescription')}`}
          fontSize='12px'
          fontColor={"#999"}
        />
        <PageLayout
          withBodyScroll
          withBodyAutoFocus
          showProgressBar={showProgressBar}
          progressBarValue={progressBarValue}
          progressBarDropDownContent={progressBarContent}
          progressBarLabel={progressBarLabel}
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={
            <ArticleMainButtonContent
              onLoading={this.onLoading}
              setProgressVisible={this.setProgressVisible}
              setProgressValue={this.setProgressValue}
              setProgressLabel={this.setProgressLabel}
            />}
          articleBodyContent={<ArticleBodyContent  onLoading={this.onLoading} isLoading={isLoading} />}
          sectionHeaderContent={
            <SectionHeaderContent
              isHeaderVisible={isHeaderVisible}
              isHeaderIndeterminate={isHeaderIndeterminate}
              isHeaderChecked={isHeaderChecked}
              onCheck={this.onSectionHeaderContentCheck}
              onSelect={this.onSectionHeaderContentSelect}
              onClose={this.onClose}
              onLoading={this.onLoading}
              isLoading={isLoading}
              setProgressValue={this.setProgressValue}
              startFilesOperations={this.startFilesOperations}
              finishFilesOperations={this.finishFilesOperations}
            />
          }
          sectionFilterContent={<SectionFilterContent onLoading={this.onLoading} />}
          sectionBodyContent={
            <SectionBodyContent
              selected={selected}
              isLoading={isLoading}
              onLoading={this.onLoading}
              onChange={this.onRowChange}
              setProgressValue={this.setProgressValue}
              startFilesOperations={this.startFilesOperations}
              finishFilesOperations={this.finishFilesOperations}
            />
          }
          sectionPagingContent={
            <SectionPagingContent onLoading={this.onLoading} />
          }
        />
      </>
    );
  }
}

const HomeContainer = withTranslation()(PureHome);

const Home = (props) => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><HomeContainer {...props} /></I18nextProvider>);
}

Home.propTypes = {
  files: PropTypes.array,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    files: state.files.files,
    folders: state.files.folders,
    selection: state.files.selection,
    selected: state.files.selected,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(
  mapStateToProps,
  { setSelected }
)(withRouter(Home));
