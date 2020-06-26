import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import {
  Backdrop,
  Heading,
  Aside,
  Row,
  RowContent,
  RowContainer,
  Text,
  Link,
  Button,
  toastr
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, api } from "asc-web-common";
import i18n from "./i18n";
import { ReactSVG } from 'react-svg'
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledFooter
} from "../StyledPanels";
import { getFileIcon, getFolderIcon, canWebEdit, isImage, isSound, isVideo } from "../../../store/files/selectors";
import { fetchFiles } from '../../../store/files/actions';
import store from "../../../store/store";

const { changeLanguage } = commonUtils;

class NewFilesPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);
  }

  getItemIcon = (item, isEdit) => {
    const extension = item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px");
          isEdit && svg.setAttribute("style", "margin-left: 24px");
        }}
        src={icon}
        loading={this.svgLoader}
      />
    );
  };

  onMarkAsRead = () => {
    const { folderId, onClose, setNewFilesCount } = this.props;
    api.files
      .markAsRead([folderId], [])
      .then(() => setNewFilesCount(folderId, 0))
      .catch(err => toastr.error(err))
      .finally(() => onClose());
  };

  onNewFilesClick = item => {
    const { setNewFilesCount, onClose, onLoading, folderId } = this.props;
    const folderIds = [];
    const fileId = [];
    const isFile = item.fileExst;

    isFile ? fileId.push(item.id) : folderIds.push(item.id);

    onLoading(true);

    api.files.markAsRead(folderIds, fileId)
      .then(() => {
        setNewFilesCount(folderId);
        this.onFilesClick(item);
      })
      .catch(err => toastr.error(err))
      .finally(() => {
        !isFile && onClose();
        onLoading(false);
      });
  }

  onFilesClick = item => {
    const { id, fileExst, viewUrl } = item;
    const { filter, /*onMediaFileClick*/ } = this.props;
    if (!fileExst) {

      fetchFiles(id, filter, store.dispatch)
        .catch(err => toastr.error(err))
    } else {
      if (canWebEdit(fileExst)) {
        return window.open(`./doceditor?fileId=${id}`, "_blank");
      }

      const isOpenMedia = isImage(fileExst) || isSound(fileExst) || isVideo(fileExst);

      if (isOpenMedia) {
        //onMediaFileClick(id);
        return;
      }

      return window.open(viewUrl, "_blank");
    }
  };


  render() {
    //console.log("NewFiles panel render");
    const { t, visible, onClose, files } = this.props;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop onClick={onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledHeaderContent className="files-operations-panel">
              <Heading size="medium" truncate>
                {t("NewFiles")}
              </Heading>
            </StyledHeaderContent>
            <StyledBody className="files-operations-body">
              <RowContainer useReactWindow manualHeight="83vh">
                {files.map((file) => {
                  const element = this.getItemIcon(file);
                  return (
                    <Row key={file.id} element={element}>
                      <RowContent onClick={this.onNewFilesClick.bind(this, file)}>
                        <Link
                          containerWidth="100%"
                          type="page"
                          fontWeight="bold"
                          color="#333"
                          isTextOverflow
                          truncate
                          title={file.title}
                          fontSize="14px"
                        >
                          {file.title}
                        </Link>

                        <></>
                        <Text fontSize="12px" containerWidth="auto">
                          {file.checked && t("ConvertInto")}
                        </Text>
                      </RowContent>
                    </Row>
                  );
                })}
              </RowContainer>
            </StyledBody>
            <StyledFooter>
              <Button
                label={t("MarkAsRead")}
                size="big"
                primary
                onClick={this.onMarkAsRead}
              />
              <Button
                className="sharing_panel-button"
                label={t("CloseButton")}
                size="big"
                onClick={onClose}
              />
            </StyledFooter>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

NewFilesPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
};

const NewFilesPanelContainerTranslated = withTranslation()(
  NewFilesPanelComponent
);

const NewFilesPanel = (props) => (
  <NewFilesPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {};
};

export default connect(mapStateToProps, {})(withRouter(NewFilesPanel));
