import React, { memo } from "react";
import { withRouter } from "react-router";
// import { useTranslation } from 'react-i18next';
import { connect } from "react-redux";
import styled from 'styled-components';
import { Text, Button, Avatar, toastr } from 'asc-web-components';
import { api } from 'asc-web-common';
import { toEmployeeWrapper } from "../../../../../store/people/selectors";
import { importUsers } from "../../../../../store/people/actions";
import { FixedSizeList as List, areEqual } from 'react-window';
import AutoSizer from 'react-virtualized-auto-sizer';

const LoadCsvWrapper = styled.div`
  margin-top: 24px;
  margin-bottom: 24px;
  width: 100%;
  height: 400px;
  max-width: 1024px;
`;

const SelectSourceWrapper = styled.div`
  margin-top: 24px;
`;

const StyledFileInput = styled.div`
  position: relative;
  width: 100%;
  max-width: 400px;
  height: 36px;

  input[type="file"] {
    height: 100%;
    font-size: 200px;
    position: absolute;
    top: 0;
    right: 0;
    opacity: 0;
  }
`;

const StyledFieldContainer = styled.div`
  position: relative;

  @media (min-width:768px) {
    margin-top: 14px;
  }
`;

const StyledAvatar = styled.div`
  float: left;

  @media (max-width:768px) {
    margin-top: 8px;
  }
`;

const StyledField = styled.div`
  line-height: 14px;
  min-width: 100px;
  padding: 4px 16px;

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  @media (min-width:768px) {
    margin-top: 4px;
    display: inline-block !important;
  }
`;

const StyledProgress = styled.div`
  position: absolute;
  max-width: 100%;
  width: ${props => props.completed && `${props.completed}%`};
  height: 100%;
  top: 0px;
  background-color: #7ACE9B;
  opacity: 0.3;
  border-radius: 3px;
  z-index: -1;
  transition-property: width;
  transition-duration: 1s;
`;

class ImportRow extends React.PureComponent {
  render() {
    const { items, completed } = this.props;

    const fullName = items[0] + ' ' + items[1];
    const firstName = items[0];
    const lastName = items[1];
    const email = items[2];

    return (
      <StyledFieldContainer key={email}>
        <StyledAvatar>
          <Avatar size='small' role='user' userName={fullName} />
        </StyledAvatar>
        <StyledField style={{ display: 'inline-block' }}>{firstName}</StyledField>
        <StyledField style={{ display: 'inline-block' }}>{lastName}</StyledField>
        <StyledField style={{ display: 'block' }}>{email}</StyledField>
        <StyledProgress completed={completed} />
      </StyledFieldContainer>
    )
  }
}

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      splittedLines: [],
      importing: false
    }
  }

  getAsText = (fileToRead) => {
    if (!fileToRead) return;
    
    let reader = new FileReader();
    reader.readAsText(fileToRead);
    reader.onload = this.loadHandler;
    reader.onerror = this.errorHandler;
  }

  loadHandler = (event) => {
    const csv = event.target.result;
    this.processData(csv);
  }

  errorHandler = (event) => {
    if (event.target.error.name === 'NotReadableError') {
      alert(event.target.error.name);
    }
  }

  processData = (csv) => {
    const allTextLines = csv.split(/\r\n|\n/);
    const splittedLines = allTextLines.map(line => line.split(','));
    const filteredLines = splittedLines.filter(line => (line[0].length > 0) && line);

    this.setState({
      splittedLines: filteredLines
    });
  }

  createRows = rows => rows.map(data => {
    return (
      <ImportRow items={data} />
    );
  });

  renderRow = memo(({ data, index, style }) => {
    return (
      <div style={style}>
        {data[index]}
      </div>
    )
  }, areEqual);

  prepareUsersData = data => {
    const { splittedLines } = this.state;
    const rawUsers = splittedLines || data;

    const preparedUsers = rawUsers.map(user => {
      const userObj = {};

      userObj.firstName = user[0];
      userObj.lastName = user[1];
      userObj.email = user[2];

      return toEmployeeWrapper(userObj);
    });

    return preparedUsers;
  }

  makeChunks = (data, size = 50) => {
    let temp = [];

    for (let i = 0; i < data.length; i += size) {
      temp.push(data.slice(i, i + size));
    }

    return temp;
  }

  runImport = users => {
    const chunks = this.makeChunks(users);

    chunks.map(async chunk => { //TODO: do something better than an asynchronous carnival
      for (const user of chunk) {
        await api.people.createUser(user);
      }
    });
    
    this.setState({
      importing: false
    });
  }

  onImportClick = () => {
    const users = this.prepareUsersData();

    this.setState({
      importing: true
    });

    this.runImport(users);
  }

  render() {
    const { splittedLines, importing } = this.state;
    // const { t } = useTranslation();
    const rows = this.createRows(splittedLines);
    const isImportEnabled = true;

    const renderList = ({ height, width }) => {
      const itemHeight = (width < 768) ? 56 : 36;
      return (
        <List
          className="List"
          height={height}
          width={width}
          itemSize={itemHeight}
          itemCount={rows.length}
          itemData={rows}
        >
          {this.renderRow}
        </List>
      )
    }

    return (
      <>
        <Text fontSize='18px' >
          Functionality at development stage.
        </Text>
        <br />
        <Text fontSize='14px' >
          Files are formatted according to <b>CSV RFC 4180</b> rules. <br />
          <b>Column Order:</b> FirstName, LastName, Email. <br />
          Comma delimiter, strings in unix format. <br />
        </Text>
        <SelectSourceWrapper>
          <StyledFileInput>
            <Button size='big' primary={true} scale={true} label="Upload CSV" isDisabled={!isImportEnabled} />
            {isImportEnabled &&
              <input type="file" name="file" onChange={(e) => this.getAsText(e.target.files[0])} accept='.csv' />
            }
          </StyledFileInput>
        </SelectSourceWrapper>
        <br />
        <Text fontSize='14px' >
          Ready for import: {splittedLines.length} of {splittedLines.length}
        </Text>
        <LoadCsvWrapper>
          <AutoSizer>
            {renderList}
          </AutoSizer>
        </LoadCsvWrapper>
        <StyledFileInput>
          <Button size='big' primary={true} scale={true} onClick={this.onImportClick} isDisabled={importing || !splittedLines.length} label="Start import" />
        </StyledFileInput>
      </>
    );
  }
}

function mapStateToProps(state) {
  return {
  };
}

export default connect(mapStateToProps, {})(withRouter(SectionBodyContent));
