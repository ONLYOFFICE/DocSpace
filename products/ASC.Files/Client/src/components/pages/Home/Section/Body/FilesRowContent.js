
import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import styled from "styled-components";
import { RowContent, Link, Text, Icons, Badge, TextInput, Button, toastr } from "asc-web-components";
import { renameFolder, updateFile } from '../../../../../store/files/actions';

class FilesRowContent extends React.PureComponent {

  constructor(props) {
    super(props);

    const titleWithoutExt = props.item.fileExst
      ? props.item.title.split('.').slice(0, -1).join('.')
      : props.item.title;

    this.state = {
      itemTitle: titleWithoutExt,
      editingId: props.editingId
    };
  }

  onClickUpdateFile = () => {
    const { editingId, updateFile, renameFolder, item } = this.props;
    const { itemTitle } = this.state;

    this.setState({ editingId: -1 }, () => {
      item.fileExst
        ? updateFile(editingId, itemTitle)
        : renameFolder(editingId, itemTitle);
    });
  };

  /* onKeyPress = (event) => {
    if (event.key === "Enter") {
      this.onClickUpdateFile();
    }
  }; */

  componentDidMount() {
    /* window.addEventListener('keydown', this.onKeyPress);
    window.addEventListener('keyup', this.onKeyPress); */
  }

  componentDidUpdate(prevProps) {
    const { editingId } = this.props;

    if (editingId !== prevProps.editingId) {
      this.setState({ editingId })
    }
  }

  componentWillUnmount() {
    /* window.removeEventListener('keydown', this.onKeyPress);
    window.removeEventListener('keyup', this.onKeyPress); */
  }

  renameTitle = e => {
    this.setState({ itemTitle: e.target.value });
  }

  onClickCancelUpdateFile = () => {
    this.setState({ editingId: -1 });
  }

  render() {
    const { item } = this.props;
    const { itemTitle, editingId } = this.state;
    const {
      contentLength,
      created,
      createdBy,
      fileExst,
      filesCount,
      fileStatus,
      foldersCount,
      id,
      title,
      version
    } = item;

    const SimpleFilesRowContent = styled(RowContent)`
    .badge-ext {
      margin-left: -8px;
      margin-right: 8px;
    }

    .badge {
      margin-right: 8px;
    }

    .badges {
      display: flex;
      align-items: center;
    }
    `;

    const EditingWrapper = styled.div`
      width: 100%;
      display: inline-flex;

      .edit-button {
        margin-left: 8px;
      }

      .edit-ok-icon {
        margin-top: -4px;
        width: 16px;
        height: 16px;
      }

      .edit-cancel-icon {
        margin-top: -4px;
        width: 14px;
        height: 14px;
      }
    `;

    const titleWithoutExt = fileExst
      ? title.split('.').slice(0, -1).join('.')
      : title;

    const fileOwner = (this.props.viewer.id === createdBy.id && "Me") || createdBy.displayName;
    const createdDate = new Date(created).toLocaleString("EN-US");
    const notConverted = ['.pdf', '.zip', '.mp3', '.mp4'];
    const canEdit = fileExst && notConverted.includes(fileExst) ? false : true;

    const okIcon = <Icons.CheckIcon
      className='edit-ok-icon'
      size='scale'
      isfill={true}
      color='#A3A9AE'
    />;

    const cancelIcon = <Icons.CrossIcon
      className='edit-cancel-icon'
      size='scale'
      isfill={true}
      color='#A3A9AE'
    />;

    const isEdit = id === editingId;

    return isEdit
      ? (<EditingWrapper>
        <TextInput
          name='title'
          scale={true}
          value={itemTitle}
          tabIndex={1}
          isAutoFocussed={true}
          onChange={this.renameTitle}
        />
        <Button
          className='edit-button'
          size='medium'
          isDisabled={false}
          onClick={this.onClickUpdateFile}
          icon={okIcon}
        />
        <Button
          className='edit-button'
          size='medium'
          isDisabled={false}
          onClick={this.onClickCancelUpdateFile}
          icon={cancelIcon}
        />
      </EditingWrapper>)
      : (
        <SimpleFilesRowContent
          sideColor="#333"
          isFile={fileExst}
        >
          <Link
            type='page'
            title={titleWithoutExt}
            fontWeight="bold"
            onClick={() => { }}
            fontSize='15px'
            color="#333"
            isTextOverflow={true}
          >
            {titleWithoutExt}
          </Link>
          <div>
            {fileExst &&
              <div className='badges'>
                <Text
                  className='badge-ext'
                  as="span"
                  color="#A3A9AE"
                  fontSize='15px'
                  fontWeight={600}
                  title={fileExst}
                  truncate={true}
                >
                  {fileExst}
                </Text>
                {fileStatus === 4 &&
                  <Icons.FileActionsConvertIcon
                    className='badge'
                    size='small'
                    isfill={true}
                    color='#A3A9AE'
                  />
                }
                {canEdit &&
                  <Icons.AccessEditIcon
                    className='badge'
                    size='small'
                    isfill={true}
                    color='#A3A9AE'
                  />
                }
                {fileStatus === 1 &&
                  <Icons.FileActionsConvertEditDocIcon
                    className='badge'
                    size='small'
                    isfill={true}
                    color='#3B72A7'
                  />
                }
                {false &&
                  <Icons.FileActionsLockedIcon
                    className='badge'
                    size='small'
                    isfill={true}
                    color='#3B72A7'
                  />
                }
                {version > 1 &&
                  <Badge
                    className='badge-version'
                    backgroundColor="#A3A9AE"
                    borderRadius="11px"
                    color="#FFFFFF"
                    fontSize="10px"
                    fontWeight={800}
                    label={`Ver.${version}`}
                    maxWidth="50px"
                    onClick={() => { }}
                    padding="0 5px"
                  />
                }
                {fileStatus === 2 &&
                  <Badge
                    className='badge-version'
                    backgroundColor="#ED7309"
                    borderRadius="11px"
                    color="#FFFFFF"
                    fontSize="10px"
                    fontWeight={800}
                    label={`New`}
                    maxWidth="50px"
                    onClick={() => { }}
                    padding="0 5px"
                  />
                }
              </div>
            }
          </div>
          <Text
            containerWidth='10%'
            as="div"
            color="#333"
            fontSize='12px'
            fontWeight={600}
            title={fileOwner}
            truncate={true}
          >
            {fileOwner}
          </Text>
          <Link
            containerWidth='12%'
            type='page'
            title={createdDate}
            fontSize='12px'
            fontWeight={400}
            color="#333"
            onClick={() => { }}
            isTextOverflow={true}
          >
            {`Created: ${createdDate}`}
          </Link>
          <Text
            containerWidth='10%'
            as="div"
            color="#333"
            fontSize='12px'
            fontWeight={600}
            title=''
            truncate={true}
          >
            {fileExst
              ? contentLength
              : `Dcs: ${filesCount} / Flds: ${foldersCount}`}
          </Text>
        </SimpleFilesRowContent>
      )
  }
};

export default connect(null, { updateFile, renameFolder })(
  withRouter(FilesRowContent)
);