require 'rubygems'
require 'sinatra'

post '/' do
  puts params.inspect
  "POST OK - params: #{params.inspect}"
end
