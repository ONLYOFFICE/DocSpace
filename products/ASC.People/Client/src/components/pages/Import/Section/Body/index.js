import React from "react";
import { withRouter } from "react-router";
// import { useTranslation } from 'react-i18next';
import { connect } from "react-redux";
import styled from 'styled-components';
import { Text, Button, Avatar, RowContainer, Icons, toastr } from 'asc-web-components';

const LoadCsvWrapper = styled.div`
  margin-top: 24px;
  width: 100%;
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

  @media (min-width:769px) {
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
  width: ${props => props.progress && `${props.progress}%`};
  height: 100%;
  top: 0px;
  background-color: #7ACE9B;
  opacity: 0.3;
  border-radius: 3px;
  z-index: -1;
  transition-property: width;
  transition-duration: 2s;
`;

class ImportRow extends React.Component {
  render() {
    const { items, completion } = this.props;

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
        <StyledProgress progress={completion || 0} />
      </StyledFieldContainer>
    )
  }
}

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      splittedLines: [],
      completion: 0
    }
  }

  getAsText = (fileToRead) => {
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

    const t0 = performance.now();

    const splittedLines = allTextLines.map(line => line.split(','));
    const filteredLines = splittedLines.filter(line => (line[0].length > 0) && line);

    const t1 = performance.now();
    toastr.success(`Parse lines took ${(t1 - t0).toFixed(0)} milliseconds.`);

    this.setState({
      splittedLines: filteredLines
    });
  }

  createRows = rows => rows.map(data => (<ImportRow items={data} />));

  setCompleted = value => this.setState({ completion: value });

  render() {
    const { splittedLines, completion } = this.state;
    // const { t } = useTranslation();
    const t0 = performance.now();

    const rows = this.createRows(splittedLines);

    const t1 = performance.now();
    splittedLines.length && toastr.success(`Render rows took ${(t1 - t0).toFixed(0)} milliseconds.`);

    return (
      <>
        <Text.Body fontSize={18} >
          Select data source
        </Text.Body>
        <br />
        <Text.Body fontSize={14} >
          Functionality at development stage. <br />
          Files are formatted according to CSV RFC rules. <br />
          Column Order: FirstName, LastName, Email. <br />
          Comma delimiter, strings in unix format. <br />
        </Text.Body>
        <SelectSourceWrapper>
          <StyledFileInput>
            <Button size='big' primary={true} scale={true} label="Upload CSV" />
            <input type="file" name="file" onChange={(e) => this.getAsText(e.target.files[0])} accept='.csv' />
          </StyledFileInput>
        </SelectSourceWrapper>
        <br />
        <Text.Body fontSize={14} >
          Ready for import: {`${splittedLines.length} of ${splittedLines.length}`}
        </Text.Body>
        <LoadCsvWrapper>
          <RowContainer manualHeight='400px'>
            {rows}
          </RowContainer>
        </LoadCsvWrapper>
      </>
    );
  }
}

function mapStateToProps(state) {
  return {
  };
}

export default connect(mapStateToProps)(withRouter(SectionBodyContent));
