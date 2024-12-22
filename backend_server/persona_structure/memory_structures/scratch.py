"""
Author: Joon Sung Park (joonspk@stanford.edu)

File: scratch.py
Description: Defines the short-term memory module for generative agents.
"""
import datetime
import json
import sys
sys.path.append('../../')


class Scratch:
  def __init__(self, f_saved):
    # Current x,y tile coordinate of the persona.
    self.curr_tile = None
    self.character = None


  def save(self, out_json):
    """
    Save persona's scratch.

    INPUT:
      out_json: The file where we wil be saving our persona's state.
    OUTPUT:
      None
    """
    scratch = dict()
    scratch["curr_tile"] = self.curr_tile
    scratch["character"] = self.character
    with open(out_json, "w") as outfile:
      json.dump(scratch, outfile, indent=2)
























