import React, { Component } from 'react';

export class NumberAdder extends Component {
  static displayName = NumberAdder.name;

  constructor(props) {
    super(props);
    this.state = { firstNumber: 0, secondNumber: 0, sum: null };
    this.handleFirstNumberChange = this.handleFirstNumberChange.bind(this);
    this.handleSecondNumberChange = this.handleSecondNumberChange.bind(this);
    this.calculateSum = this.calculateSum.bind(this);
  }

  handleFirstNumberChange(event) {
    this.setState({ firstNumber: parseFloat(event.target.value) || 0 });
  }

  handleSecondNumberChange(event) {
    this.setState({ secondNumber: parseFloat(event.target.value) || 0 });
  }

  calculateSum() {
    this.setState({
      sum: this.state.firstNumber + this.state.secondNumber
    });
  }

  render() {
    return (
      <div>
        <h1>Number Adder</h1>

        <p>This is a simple example of a React component with number inputs.</p>

        <div>
          <input type="number" value={this.state.firstNumber} onChange={this.handleFirstNumberChange} placeholder="Enter first number" />
          <input type="number" value={this.state.secondNumber} onChange={this.handleSecondNumberChange} placeholder="Enter second number" />
        </div>


        {this.state.sum !== null && (
            <p aria-live="polite">Sum: <strong>{this.state.sum}</strong></p>
        )}
        <button className="btn btn-primary" onClick={this.calculateSum}>Calculate Sum</button>
      </div>
    );
  }
}