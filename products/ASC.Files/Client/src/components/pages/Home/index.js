import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
//import { RequestLoader } from "asc-web-components";
import { PageLayout, utils, api, store } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  ArticleBodyContent,
  ArticleHeaderContent,
  ArticleMainButtonContent,
} from "../../Article";
import {
  SectionBodyContent,
  SectionFilterContent,
  SectionHeaderContent,
  SectionPagingContent,
} from "./Section";
import {
  fetchFiles,
  setDragging,
  setIsLoading,
  setFirstLoad,
  startUpload,
  setSelections,
} from "../../../store/files/actions";
import {
  getConvertDialogVisible,
  getSelectedFolderId,
  getFileActionId,
  getFilter,
  getPrimaryProgressData,
  getSecondaryProgressData,
  getTreeFolders,
  getViewAs,
  getIsLoading,
  getIsRecycleBinFolder,
  getDragging,
  getSharePanelVisible,
  getFirstLoad,
} from "../../../store/files/selectors";

import { ConvertDialog } from "../../dialogs";
import { SharingPanel } from "../../panels";
import { createI18N } from "../../../helpers/i18n";
import { getFilterByLocation } from "../../../helpers/converters";
const i18n = createI18N({
  page: "Home",
  localesPath: "pages/Home",
});
const { changeLanguage } = utils;
const { FilesFilter } = api;
const { getSettingsHomepage } = store.auth.selectors;

class PureHome extends React.Component {
  componentDidMount() {
    const { fetchFiles, homepage, setIsLoading, setFirstLoad } = this.props;

    const reg = new RegExp(`${homepage}((/?)$|/filter)`, "gm"); //TODO: Always find?
    const match = window.location.pathname.match(reg);
    let filterObj = null;

    if (match && match.length > 0) {
      filterObj = getFilterByLocation(window.location);

      if (!filterObj) {
        filterObj = FilesFilter.getDefault();
        const folderId = filterObj.folder;
        setIsLoading(true);
        fetchFiles(folderId, filterObj).finally(() => {
          setIsLoading(false);
          setFirstLoad(false);
        });

        return;
      }
    }

    if (!filterObj) return;

    let dataObj = { filter: filterObj };

    if (filterObj && filterObj.authorType) {
      const authorType = filterObj.authorType;
      const indexOfUnderscore = authorType.indexOf("_");
      const type = authorType.slice(0, indexOfUnderscore);
      const itemId = authorType.slice(indexOfUnderscore + 1);

      if (itemId) {
        dataObj = {
          type,
          itemId,
          filter: filterObj,
        };
      } else {
        filterObj.authorType = null;
        dataObj = { filter: filterObj };
      }
    }

    if (!dataObj) return;

    const { filter, itemId, type } = dataObj;
    const newFilter = filter ? filter.clone() : FilesFilter.getDefault();
    const requests = [Promise.resolve(newFilter)];

    if (type === "group") {
      requests.push(api.groups.getGroup(itemId));
    } else if (type === "user") {
      requests.push(api.people.getUserById(itemId));
    }

    setIsLoading(true);
    Promise.all(requests)
      .catch((err) => {
        Promise.resolve(FilesFilter.getDefault());
        console.warn("Filter restored by default", err);
      })
      .then((data) => {
        const filter = data[0];
        const result = data[1];
        if (result) {
          const type = result.displayName ? "user" : "group";
          const selectedItem = {
            key: result.id,
            label: type === "user" ? result.displayName : result.name,
            type,
          };
          filter.selectedItem = selectedItem;
        }

        if (filter) {
          const folderId = filter.folder;
          //console.log("filter", filter);
          return fetchFiles(folderId, filter);
        }

        return Promise.resolve();
      })
      .finally(() => {
        setIsLoading(false);
        setFirstLoad(false);
      });
  }

  onDrop = (files, uploadToFolder) => {
    const {
      t,
      currentFolderId,
      startUpload,
      setDragging,
      dragging,
    } = this.props;
    const folderId = uploadToFolder ? uploadToFolder : currentFolderId;

    dragging && setDragging(false);
    startUpload(files, folderId, t);
  };

  componentDidUpdate(prevProps) {
    if (this.props.isLoading !== prevProps.isLoading) {
      if (this.props.isLoading) {
        utils.showLoader();
      } else {
        utils.hideLoader();
      }
    }
  }

  render() {
    //console.log("Home render");
    const {
      primaryProgressData,
      secondaryProgressData,
      viewAs,
      convertDialogVisible,
      sharingPanelVisible,
      fileActionId,
      isRecycleBin,
      firstLoad,
    } = this.props;

    return (
      <>
        {convertDialogVisible && (
          <ConvertDialog visible={convertDialogVisible} />
        )}

        {sharingPanelVisible && <SharingPanel />}
        <PageLayout
          withBodyScroll
          withBodyAutoFocus={!isMobile}
          uploadFiles={!isRecycleBin}
          onDrop={this.onDrop}
          setSelections={this.props.setSelections}
          onMouseMove={this.onMouseMove}
          showPrimaryProgressBar={primaryProgressData.visible}
          primaryProgressBarValue={primaryProgressData.percent}
          primaryProgressBarIcon={primaryProgressData.icon}
          showPrimaryButtonAlert={primaryProgressData.alert}
          showSecondaryProgressBar={secondaryProgressData.visible}
          secondaryProgressBarValue={secondaryProgressData.percent}
          secondaryProgressBarIcon={secondaryProgressData.icon}
          showSecondaryButtonAlert={secondaryProgressData.alert}
          viewAs={viewAs}
          hideAside={
            !!fileActionId ||
            primaryProgressData.visible ||
            secondaryProgressData.visible
          }
          isLoaded={!firstLoad}
        >
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>

          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>

          <PageLayout.ArticleBody>
            <ArticleBodyContent onTreeDrop={this.onDrop} />
          </PageLayout.ArticleBody>
          <PageLayout.SectionHeader>
            <SectionHeaderContent />
          </PageLayout.SectionHeader>

          <PageLayout.SectionFilter>
            <SectionFilterContent />
          </PageLayout.SectionFilter>

          <PageLayout.SectionBody>
            <SectionBodyContent
              isMobile={isMobile}
              onChange={this.onRowChange}
              onDropZoneUpload={this.onDrop}
            />
          </PageLayout.SectionBody>

          <PageLayout.SectionPaging>
            <SectionPagingContent />
          </PageLayout.SectionPaging>
        </PageLayout>
      </>
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
  history: PropTypes.object.isRequired,
};

function mapStateToProps(state) {
  return {
    convertDialogVisible: getConvertDialogVisible(state),
    currentFolderId: getSelectedFolderId(state),
    fileActionId: getFileActionId(state),
    filter: getFilter(state),
    isRecycleBin: getIsRecycleBinFolder(state),
    primaryProgressData: getPrimaryProgressData(state),
    secondaryProgressData: getSecondaryProgressData(state),
    treeFolders: getTreeFolders(state),
    viewAs: getViewAs(state),
    isLoading: getIsLoading(state),
    homepage: getSettingsHomepage(state),
    dragging: getDragging(state),
    firstLoad: getFirstLoad(state),
    sharingPanelVisible: getSharePanelVisible(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setDragging: (dragging) => dispatch(setDragging(dragging)),
    startUpload: (files, folderId, t) =>
      dispatch(startUpload(files, folderId, t)),
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
    setFirstLoad: (firstLoad) => dispatch(setFirstLoad(firstLoad)),
    fetchFiles: (folderId, filter) => dispatch(fetchFiles(folderId, filter)),
    setSelections: (items) => dispatch(setSelections(items)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(withRouter(Home));
